using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.Helpers;
using System.Windows.Forms;
using Exportador.Interface;

namespace Exportador.RH.Funcionario
{
    public class ExportadorFuncionario : IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private bool _debugMode;

        #endregion

        #region Properties

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        public int RecordsCount
        {
            get
            {
                return _recordsToReturn;
            }
            set
            {
                _recordsToReturn = value;
            }
        }

        public bool DebugMode
        {
            get { return _debugMode; }
            set { _debugMode = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorFuncionario()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorFuncionario(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorFuncionario(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryFunc = @"
                                     select
                                        funcionario.numcad as Id

                                        --campos obrigatórios
      ,case when funcionario.tipcol=2 
		and funcionario.usu_terati='N'
		and LEN(case when funcionario.codcha='' then funcionario.numcad else funcionario.codcha end)<5
		then 
              '6'+REPLICATE('0', 4 - LEN(
                                          case when funcionario.codcha='' then funcionario.numcad else funcionario.codcha end
                                          ))+ 
                                          RTrim(case when funcionario.codcha='' then funcionario.numcad else funcionario.codcha end)
      else 
              CAST(funcionario.numcad as varchar(50))
      end
      as Chapa
                                        ,funcionario.nomfun as Nome
                                        ,funcionario.datnas as Nascimento
                                        ,ficha.estnas as EstadoNatal
                                        ,funcionario.tipsex as Sexo
                                        ,grauInstrucao.desgra as GrauInstrucao
                                        ,funcionario.numcpf as CPF
                                        ,ficha.ficreg as FichaRegistro
                                        ,funcionario.datadm as DataAdmissao
                                        ,funcionario.codcar as CodFuncao
                                        ,case when funcionario.tipcol=2 then
                                            'Autônomo'
                                        else                                            
                                            case funcionario.tipcon
                                                when 1 then 'Empregado'
                                                when 2 then 'Diretor'
                                                when 3 then 'Trabalhador Rural'
                                                when 4 then 'Aposentado'
                                                when 5 then 'Estagiário'
                                                when 6 then 'Aprendiz'
                                                when 7 then 'Prazo Determinado'
                                                when 8 then 'Diretor Aposentado'
                                                when 9 then 'Agente Público'
                                                when 10 then 'Professor'
                                                when 11 then 'Cooperado'
                                                when 12 then 'Trabalhador Doméstico'
                                                when 13 then 'Professor Prazo Determinado'
                                            end 
                                        end
                                        as TipoFuncionario
                                        ,funcionario.valsal as Salario
                                        ,'01.'+secao.codloc as CodSecao
                                        ,situacao.dessit as Situacao
                                        ,funcionario.apefun as Apelido	
                                        ,case funcionario.estciv
                                            when 1 then 'Solteiro'
                                            when 2 then 'Casado'
                                            when 3 then 'Divorciado'
                                            when 4 then 'Viúvo'
                                            when 5 then 'Concubinato'
                                            when 6 then 'Separado'
                                            when 7 then 'União Estável'
                                            when 9 then 'Outros'
                                        end		 
                                        as EstadoCivil	
                                        ,nacionalidade.desnac as Nacionalidade	
                                        ,ficha.endrua as Rua
                                        ,ficha.endnum as Numero
                                        ,ficha.endcpl as Complemento
                                        ,bairroEndereco.nombai as Bairro
                                        ,ficha.codest as Estado
                                        ,cidadeEndereco.nomcid as Cidade
                                        ,ficha.endcep as CEP
                                        ,paisEndereco.nompai as Pais
                                        ,ficha.regcon as RegistroProfissional	
                                        ,case when ficha.dddtel <> 0 then '('+CAST(ficha.dddtel as varchar(2))+')' end + ficha.numtel as Telefone1
                                        ,case when ficha.nmddd2 <> 0 then '('+CAST(ficha.nmddd2 as varchar(2))+')' end + ficha.nmtel2 as Telefone2
                                        ,ficha.numcid as CarteiraIdentidade
                                        ,ficha.estcid as UFCartIdentidade
                                        ,ficha.emicid as OrgaoEmissorCartIdentidade
                                        ,ficha.dexcid as EmissaoCartIdentidade
                                        ,ficha.numele as TituloEleitor
                                        ,ficha.zonele as ZonaVotacao
                                        ,ficha.secele as SecaoVotacao
                                        ,funcionario.numctp as CarteiraTrabalho
                                        ,funcionario.serctp as SerieCarteiraTrabalho
                                        ,funcionario.estctp as UFCarteiraTrabalho
                                        ,funcionario.dexctp as EmissaoCarteiraTrabalho
                                        ,ficha.numcnh as CarteiraMotorista
                                        ,ficha.catcnh as TipoCarteiraMotorista
                                        ,ficha.vencnh as VencCartMotorista
                                        ,ficha.numres as CertifReservista
                                        ,ficha.catres as CategMilitar
                                        ,cidadeNatural.nomcid as Naturalidade
                                        ,funcionario.datche as ChegadaAoBrasil	
                                        , case when funcionario.tipcon=10 then
												'Professor'
											else
												case funcionario.perpag
													when 'M' then 'Mensal'
													when 'S' then 'Semanal'
													when 'Q' then 'Quinzenal'
												end                                    
											end 
                                        as TipoRecebimento
                                        ,REPLICATE('0', 4 - LEN(sind_fun.codsin))+ RTrim(sind_fun.codsin) as CodSindicato
                                        ,case when funcionario.tipcol=2 then 'N' else
                                            funcionario.socsin
                                        end as MembroSindical
                                        ,CAST(escala.hormes/60 AS VARCHAR(50)) +':'+ REPLICATE('0', 2 - LEN(CAST(escala.hormes%60 AS VARCHAR(50))))+ RTrim(CAST(escala.hormes%60 AS VARCHAR(50))) as Jornada
                                        ,escala.codesc as CodHorario
                                        ,case when funcionario.tipcol=2 then 'Não optante'
                                        else
		                                    case funcionario.tipopc
			                                    when 'S' then 'Optante'
			                                    when 'N' then 'Não optante'
		                                    end 
	                                    end
                                        as SituacaoFGTS
                                        ,funcionario.datopc as DataOpcaoFGTS
                                        ,case funcionario.visest
                                            when 1 then 'Temporário'
                                            when 2 then 'Permanente'
                                            when 3 then 'Turista'
                                            when 4 then 'Oficial'
                                            when 9 then 'Outros'
                                        end as TipoVisto
                                        ,funcionario.codban as CodBancoPgto
                                        ,funcionario.CodAge as CodAgenciaPgto
                                        ,case funcionario.pagsin 
		                                    when 'S' then 'J'
		                                    when 'N' then 'N'
		                                    when 'M' then 'L'
	                                    else 'N'
	                                    end as ContribuicaoSindical
	                                    ,case funcionario.tipadm
		                                    when 1 then 'P'
		                                    when 2 then 'R'
	                                    else 'O'
	                                    end as TipoAdmissao
	                                    ,case when funcionario.tipcol=1 then
		                                    case funcionario.sitafa
			                                    when 1 then '1'
			                                    when 2 then '1'
			                                    when 3 then '5'
			                                    when 4 then '2'
			                                    when 6 then '4'
			                                    when 7 then '6'
			                                    when 8 then '6'
			                                    when 14 then '1'
			                                    when 208 then '6'
			                                    when 659 then '6'
		                                    end 
	                                    else 'N'
	                                    end
	                                    as SituacaoRAIS
                                        ,case funcionario.codfil when 5 then 1 end as CodFilial
                                        ,funcionario.raccor as RacaCor
                                        ,case funcionario.tipcol
                                            when 1 then 1
                                            when 2 then 13
                                        end as CodCategoria   
                                    from {schemaName}.r034fun as funcionario
                                    left join {schemaName}.r023nac as nacionalidade on nacionalidade.codnac=funcionario.codnac
                                    left join {schemaName}.r022gra as grauInstrucao on funcionario.GraIns=grauInstrucao.grains
                                    left join {schemaName}.r034cpl as ficha on ficha.numemp=funcionario.numemp
                                        and ficha.tipcol=funcionario.tipcol
                                        and ficha.numcad=funcionario.numcad
                                    left join {schemaName}.r074bai bairroEndereco on bairroEndereco.codcid=ficha.codcid
                                        and bairroEndereco.codbai=ficha.codbai
                                    left join {schemaName}.r074cid cidadeEndereco on cidadeEndereco.codcid=ficha.codcid
                                    left join {schemaName}.r074pai paisEndereco on paisEndereco.codpai=ficha.codpai
                                    left join {schemaName}.r074cid cidadeNatural on cidadeNatural.codcid=ficha.ccinas
                                    left join {schemaName}.r010sit situacao on situacao.codsit=funcionario.sitafa
                                    left join {schemaName}.r024car cargo on cargo.codcar=funcionario.codcar
                                    left join {schemaName}.r038hsi sind_fun on sind_fun.numemp=funcionario.numemp
                                        and sind_fun.tipcol=funcionario.tipcol
                                        and sind_fun.numcad=funcionario.numcad
                                        and sind_fun.datalt=funcionario.datsin
                                    left join {schemaName}.r006esc escala on escala.codesc=funcionario.codesc
                                    inner join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                    where secao.taborg=5
";

        #endregion

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e) 
        {
            if (error)
            {
                MessageBox.Show("Houveram erros no processo.", "Erro.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Processo concluído com sucesso.", "Sucesso.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void Exportar()
        {
            error = false;

            List<Funcionario> funcionarios = new List<Funcionario>();

            //funcionarios.AddRange(buscarFuncionarios(error));

            error = buscarFuncionarios(funcionarios);

            FileHelperEngine engine = new FileHelperEngine(typeof(Funcionario),Encoding.Default);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, funcionarios);
        }


        private bool buscarFuncionarios(List<Funcionario> lFunc)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryFunc.Replace("{schemaName}", dbName));

            IDataReader drFunc = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drFunc.Read())
            {
                Funcionario func = new Funcionario();

                try
                {
                    processedRecords++;

                    func.Id = Convert.ToInt32(drFunc["Id"]);

                    validarObrigatorios(drFunc);            

                    func.Chapa = drFunc["Chapa"].ToString();
                    func.Nome = drFunc["Nome"].ToString().RemoveSpecialChars();
                    func.Nascimento = Convert.ToDateTime(drFunc["Nascimento"]);
                    func.EstadoNatal = drFunc["EstadoNatal"].ToString();
                    func.Sexo = drFunc["Sexo"].ToString();
                    func.GrauInstrucao = BuscarGrauInstrucao(drFunc["GrauInstrucao"].ToString());
                    func.CPF = drFunc["CPF"].ToString().PadLeft(11,'0');
                    func.FichaRegistro = String.IsNullOrWhiteSpace(drFunc["FichaRegistro"].ToString().Replace("0","")) ?  Convert.ToInt32(func.Chapa) : Convert.ToInt32(drFunc["FichaRegistro"]);
                    func.DataAdmissao = Convert.ToDateTime(drFunc["DataAdmissao"]);
                    func.CodFuncao = drFunc["CodFuncao"].ToString();
                    func.TipoFuncionario = BuscarTipoFuncionario(drFunc["TipoFuncionario"].ToString());
                    func.Salario = Convert.ToDouble(drFunc["Salario"]);
                    func.CodSecao = drFunc["CodSecao"].ToString();
                    func.Situacao = BuscarSituacao(drFunc["Situacao"].ToString());
                    func.Apelido = drFunc["Apelido"].ToString().RemoveSpecialChars();
                    func.EstadoCivil = BuscarEstadoCivil(drFunc["EstadoCivil"].ToString());
                    func.Nacionalidade = BuscarNacionalidade(drFunc["Nacionalidade"].ToString());
                    func.Rua = drFunc["Rua"].ToString().RemoveSpecialChars();
                    func.Numero = drFunc["Numero"].ToString();
                    func.Complemento = drFunc["Complemento"].ToString().RemoveSpecialChars();
                    func.Bairro = drFunc["Bairro"].ToString().RemoveSpecialChars();
                    func.Estado = drFunc["Estado"].ToString().RemoveSpecialChars();
                    func.Cidade = drFunc["Cidade"].ToString().RemoveSpecialChars();
                    func.CEP = drFunc["CEP"].ToString();
                    func.Pais = drFunc["Pais"].ToString().RemoveSpecialChars();
                    func.RegistroProfissional = drFunc["RegistroProfissional"].ToString();
                    func.Telefone1 = drFunc["Telefone1"].ToString();
                    func.Telefone2 = drFunc["Telefone2"].ToString();
                    func.CarteiraIdentidade = drFunc["CarteiraIdentidade"].ToString();
                    func.UFCartIdentidade = drFunc["UFCartIdentidade"].ToString();
                    func.OrgaoEmissorCartIdentidade = drFunc["OrgaoEmissorCartIdentidade"].ToString();
                    func.EmissaoCartIdentidade = Convert.ToDateTime(drFunc["EmissaoCartIdentidade"]);
                    func.TituloEleitor = drFunc["TituloEleitor"].ToString();
                    func.ZonaVotacao = drFunc["ZonaVotacao"].ToString();
                    func.SecaoVotacao = drFunc["SecaoVotacao"].ToString();
                    func.CarteiraTrabalho = drFunc["CarteiraTrabalho"].ToString();
                    func.TipoCarteiraMotorista = drFunc["TipoCarteiraMotorista"].ToString();
                    func.VencCartMotorista = Convert.ToDateTime(drFunc["VencCartMotorista"]);
                    func.CertifReservista = drFunc["CertifReservista"].ToString();
                    func.CategMilitar = drFunc["CategMilitar"].ToString();
                    func.Naturalidade = drFunc["Naturalidade"].ToString().RemoveSpecialChars();
                    func.ChegadaAoBrasil = Convert.ToDateTime(drFunc["ChegadaAoBrasil"].DefaultDbNull("1900-1-1"));
                    func.TipoRecebimento = BuscarTipoRecebimento(drFunc["TipoRecebimento"].ToString());
                    func.CodSindicato = drFunc["CodSindicato"].ToString();
                    func.Jornada = drFunc["Jornada"].ToString();
                    func.CodHorario = drFunc["CodHorario"].ToString();
                    func.SituacaoFGTS = BuscarSituacaoFGTS(drFunc["SituacaoFGTS"].ToString());
                    func.DataOpcaoFGTS = Convert.ToDateTime(drFunc["DataOpcaoFGTS"]);
                    func.TipoVisto = drFunc["TipoVisto"].ToString();
                    func.MembroSindical = ConverterBoolean(drFunc["MembroSindical"].ToString());
                    func.ContribuicaoSindical = drFunc["ContribuicaoSindical"].ToString();
                    func.TipoAdmissao = drFunc["TipoAdmissao"].ToString();
                    func.SituacaoRAIS = buscarSituacaoRAIS(drFunc["SituacaoRAIS"].ToString());
                    func.VinculoRAIS = String.IsNullOrWhiteSpace(func.SituacaoRAIS) ? String.Empty : buscarVinculoRAIS(drFunc["TipoFuncionario"].ToString());
                    func.CodFilial = Convert.ToInt32(drFunc["CodFilial"]);
                    func.CorRaca = buscarRacaCor(drFunc["RacaCor"].ToString());
                    func.CodBancoPgto = BuscarBanco(drFunc["CodBancoPgto"].ToString());
                    func.CodAgenciaPgto = drFunc["CodAgenciaPgto"].ToString();
                    func.CodCategoria = Convert.ToInt32(drFunc["CodCategoria"]);

                    func.MotivoAdmissao = MotivoAdmissao.AumentoQuadro;
                    func.IndiceInicioHorario = "1"; //Deixado fixo segunda-feira...
                    func.CodOcorrencia = Ocorrencia.NuncaExpostoAAgenteNocivo;//"0"; //Deixado fixo "Nunca exposto a agentes"
                    func.DataBase = func.DataAdmissao;
                    func.SituacaoINSS = 1;//Deixado fixo como "Calcula".

                    lFunc.Add(func);                    

                }
                catch (BusinessException ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível importar o funcionário {0}, Id {1}. Motivo:{2}", func.Nome, func.Id, ex.Message));                    
                }

                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível importar o funcionário {0}, Id {1}. Motivo:{2} {3}", func.Nome, func.Id, ex.Message, ex.StackTrace));                    
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;
        }

        private int buscarRacaCor(string from)
        {
            int corRaca=0;

            switch (from)
            {
                case "1":
                    corRaca = CorRaca.Branca;
                    break;
                case "2":
                    corRaca = CorRaca.Preta;
                    break;
                case "3":
                    corRaca = CorRaca.Amarela;
                    break;
                case "4":
                    corRaca = CorRaca.Parda;
                    break;
                case "5":
                    corRaca = CorRaca.Indigena;
                    break;
            }
            
            if (corRaca == 0)
                throw new BusinessException("Funcionário não possui cor/raça cadastrada.");

            return corRaca;
        }

        /// <summary>
        /// Retorna o código da agência no RM.
        /// </summary>
        /// <param name="codBancoRM">Código do Banco no RM</param>
        /// <param name="from">Código da agência na origem.</param>
        /// <returns></returns>
        private string BuscarAgencia(string codBancoRM, string from)
        {
            throw new NotImplementedException();
        }

        private string BuscarBanco(string from)
        {
            string codBanco = "";

            //if (String.IsNullOrWhiteSpace(from.Replace("0","")))
            //    throw new BusinessException("Funcionário não possui banco associado.");

            switch (from)
            {
                case "1":
                    codBanco = "001";
                    break;
                case "275":
                    codBanco = "033";
                    break;
                case "27":
                    codBanco = "001";
                    break;
                case "104":
                    codBanco = "104";
                    break;
                case "140":
                    codBanco = "033";
                    break;
                case "38":
                    codBanco = "001";
                    break;
                case "420":
                    codBanco = "104";
                    break;
                case "748":
                    codBanco = "748";
                    break;
                case "356":
                    codBanco = "033";
                    break;
                case "1242":
                    codBanco = "033";
                    break;

                default:
                    codBanco = "";
                    break;
            }


            return codBanco;
        }

        private string BuscarGrauInstrucao(string from)
        {
            if (String.IsNullOrWhiteSpace(from))
                throw new BusinessException("Funcionário não possui grau de instrucao associado.");

            switch (from)
            {
                case "2º Grau Completo":
                    return GrauInstrucao.GinasialCompleto;
                case "Superior Completo":
                    return GrauInstrucao.SuperiorCompleto;
                case "Pós-Graduação":
                    return GrauInstrucao.PosGradCompleto;
                case "Mestrado":
                    return GrauInstrucao.MestradoCompleto;
                case "Superior Incompleto":
                    return GrauInstrucao.SuperiorIncompleto;
                case "Doutorado":
                    return GrauInstrucao.DoutoradoCompleto;
                case "1º Grau Completo":
                    return GrauInstrucao.PrimarioCompleto;
                case "2º Grau Incompleto":
                    return GrauInstrucao.GinasialIncompleto;
                case "5ª a 8ª Série Incompleta":
                    return GrauInstrucao.GinasialIncompleto;
                case "4ª Série Incompleta":
                    return GrauInstrucao.PrimarioIncompleto;
                case "4ª Série Completa":
                    return GrauInstrucao.PrimarioCompleto;
                case "Analfabeto":
                    return GrauInstrucao.Analfabeto;
                case "Ph.D.":
                    return GrauInstrucao.PosDoutoradoCompleto;

                default:
                    return String.Empty;
            }
        }

        private string BuscarNacionalidade(string from)
        {
            if (String.IsNullOrWhiteSpace(from))
                throw new BusinessException("Funcionário não possui Nacionalidade cadastrada.");


            switch (from)
            {
                case "Brasileiro":
                    return Nacionalidade.Brasileiro.ToString();
                case "Português":
                    return Nacionalidade.Portugues.ToString();
                case "Argentino":
                    return Nacionalidade.Argentino.ToString();
                case "Boliviano":
                    return Nacionalidade.Boliviano.ToString();
                case "Chileno":
                    return Nacionalidade.Chileno.ToString();
                case "Brasileiro Naturalizado":
                    return Nacionalidade.NaturalBrasileiro.ToString();
                case "Uruguaio":
                    return Nacionalidade.Uruguaio.ToString();
                case "Outros Latino-Americanos":
                    return Nacionalidade.LatinoAmericano.ToString();
                case "PERUANO":
                    return Nacionalidade.LatinoAmericano.ToString();
                case "Italiano":
                    return Nacionalidade.Italiano.ToString();
                case "Outros":
                    return Nacionalidade.Outros.ToString();
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Retorna o vínculo da RAIS do funcionário com base no seu tipo de contrato.
        /// </summary>
        /// <param name="tipoContrato">Tipo de contrato do funcionário.</param>
        /// <returns>Vínculo RAIS do funcionário.</returns>
        private string buscarVinculoRAIS(string tipoContrato)
        {
            if (String.IsNullOrWhiteSpace(tipoContrato))
                throw new BusinessException("Funcionário não possui tipo de contrato associado.");

            switch (tipoContrato)
            {
                case "Empregado":
                    return VinculoRAIS.ContrTrabExprOuTacitoPPrazoIndeterminado;
                case "Professor":
                    return VinculoRAIS.ContrTrabExprOuTacitoPPrazoIndeterminado;
                case "Estagiário":
                    return VinculoRAIS.Aprendiz;
                case "Prazo Determinado":
                    return VinculoRAIS.ContrTrabP_CLTPorTempoDeterminadoOuObraCerta;

                default:
                    return String.Empty;
            }
        }

        private string buscarSituacaoRAIS(string from)
        {
            if (String.IsNullOrWhiteSpace(from))
                throw new BusinessException("Funcionário não possui situação RAIS associado.");

            switch (from)
            {
                case "1":
                    return SituacaoRAIS.AtivNormalCRemunLicRemunCDirInteg;
                case "2":
                    return SituacaoRAIS.AfastadoPorAcidDeTrabalhoPorPeriodoSuperiorA15Dias;
                case "3":
                    return SituacaoRAIS.AfastadoPorPrestacaoDeServicoMilitar;
                case "4":
                    return SituacaoRAIS.AfastadoPorMotivoDeLicencaGestante;
                case "5":
                    return SituacaoRAIS.AfastadoPorDoencaPorPeriodoSuperiorA15Dias;
                case "6":
                    return SituacaoRAIS.ForaAtivComVagaQuadroEmpSemBenefLegisl;
                case "7":
                    return SituacaoRAIS.AfastadoPorMaisDeUmMotivo;
                case "N":
                    return SituacaoRAIS.NaoSaiNaRAIS;

                default:
                    return String.Empty;
            }
        }

        private bool? ConverterBoolean(string from)
        {
            switch (from)
            {
                case "S":
                    return true;
                case "N":
                    return false;
                default:
                    return null;
            }
        }

        private void validarObrigatorios(IDataReader drFunc)
        {
            if (String.IsNullOrWhiteSpace(drFunc["Chapa"].ToString()))
            {
                throw new BusinessException("Chapa do funcionário não cadastrada.");
            }

            if (String.IsNullOrWhiteSpace(drFunc["FichaRegistro"].ToString()))
            {
                throw new BusinessException("Ficha registro do funcionário não cadastrada.");
            }

            if (String.IsNullOrWhiteSpace(drFunc["CarteiraIdentidade"].ToString()))
            {
                throw new BusinessException("Carteira de identidade do funcionário não cadastrada.");
            }

            if ((String.IsNullOrWhiteSpace(drFunc["EstadoNatal"].ToString())) && (drFunc["TipoFuncionario"].ToString() != TipoFuncionario.Autonomo))
            {
                throw new BusinessException("Quando o funcionário for do tipo Empregado ou Professor, o campo 'Estado Natal' deve estar preenchido.");
            }

            if ((String.IsNullOrWhiteSpace(drFunc["EstadoCivil"].ToString())) && (drFunc["TipoFuncionario"].ToString() != TipoFuncionario.Autonomo))
            {
                throw new BusinessException("Quando o funcionário for do tipo Empregado ou Professor, o campo 'Estado Civil' deve estar preenchido.");
            }
            
        }

        private string BuscarSituacaoFGTS(string from)
        {
            switch (from)
            {
                case "Optante":
                    return SituacaoFGTS.Optante;
                case "Não optante":
                    return SituacaoFGTS.NaoOptante;

                default:
                    return "";
            }
        }

        private string BuscarTipoRecebimento(string from)
        {
            switch (from)
            {
                case "Mensal":
                    return TipoRecebimento.Mensalista;
                case "Semanal":
                    return TipoRecebimento.Semanalista;
                case "Quinzenal":
                    return TipoRecebimento.Quinzenalista;
                case "Professor":
                    return TipoRecebimento.ProfessorHorista;

                default:
                    return "";
            }
        }

        private string BuscarSituacao(string from)
        {
            switch (from)
            {
                case "Acidente Trabalho":
                    return SituacaoFuncionario.AfastAcidTrabalho;
                case "ATESTADO MÉDICO":
                    return SituacaoFuncionario.LicencaRemunerada;
                case "Auxilio Doenca":
                    return SituacaoFuncionario.LicencaRemunerada;
                case "Demitido":
                    return SituacaoFuncionario.Demitido;
                case "Ferias":
                    return SituacaoFuncionario.Ferias;
                case "Lic.Maternidade":
                    return SituacaoFuncionario.LicencaMaternidade;
                case "Licença s/ Remuneração":
                    return SituacaoFuncionario.LicencaSemVencimento;
                case "Sem atividades":
                    return SituacaoFuncionario.Outros;
                case "Trabalhando":
                    return SituacaoFuncionario.Ativo;
                default:
                    return "";
            }
        }

        private string BuscarEstadoCivil(string from)
        {
            switch (from)
            {
                case "Solteiro":
                    return EstadoCivil.Solteiro;
                case "Casado":
                    return EstadoCivil.Casado;
                case "Divorciado":
                    return EstadoCivil.Divorciado;
                case "Viúvo":
                    return EstadoCivil.Viuvo;
                case "Concubinato":
                    return EstadoCivil.Solteiro;
                case "Separado":
                    return EstadoCivil.Desquitado;
                case "União Estável":
                    return EstadoCivil.Solteiro;
                default:
                    return "";
            }
        }

        private string BuscarTipoFuncionario(string from)
        {
            switch (from)
            {
                case "Empregado":
                    return TipoFuncionario.Normal;
                case "Diretor":
                    return TipoFuncionario.Diretor;
                case "Trabalhador Rural":
                    return TipoFuncionario.Rural;
                case "Aposentado":
                    return "";
                case "Estagiário":
                    return TipoFuncionario.Estagiario;
                case "Aprendiz":
                    return TipoFuncionario.Aprendiz;
                case "Prazo Determinado":
                    return "";
                case "Diretor Aposentado":
                    return "";
                case "Agente Público":
                    return "";
                case "Professor":
                    return TipoFuncionario.Normal;
                case "Cooperado":
                    return "";
                case "Trabalhador Doméstico":
                    return "";
                case "Professor Prazo Determinado":
                    return "";
                case "Autônomo":
                    return TipoFuncionario.Autonomo;



                default:
                    return "";
            }
        }

    }
}
