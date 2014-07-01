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
using System.Configuration;
using Exportador.DAO;

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

        public static string _queryFunc = @"
                                    select	
                                        funcionario.numcad as Id
                                        --campos obrigatórios
	                                    ,chapa.Chapa as Chapa
                                        ,UPPER(funcionario.nomfun) as Nome
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
                                        ,funcionario.valsal as Salario,
                                        case when len(secao.codloc) = 15 and secao.taborg = '5'
		                                      then '01.' + secao.codloc
		                                      else '01.01.99.99.99.999'
                                        end as CodSecao
                                        ,case situacao.codsit
		                                    when 1 then 'A'
		                                    when 2 then 'F'
		                                    when 3 then 'P'
		                                    when 4 then 'T'
		                                    when 5 then 'M'
		                                    when 6 then 'E'
		                                    when 7 then 'D'
		                                    when 8 then 'L'
		                                    when 9 then 'R'
		                                    when 10 then 'R'
		                                    when 11 then 'U'
		                                    when 12 then 'F'
		                                    when 13 then 'V'
		                                    when 14 then 'U'
		                                    when 15 then 'U'
		                                    when 16 then 'U'
		                                    when 17 then 'D'
		                                    when 18 then 'U'
		                                    when 19 then 'U'
		                                    when 20 then 'U'
		                                    when 24 then 'U'
		                                    when 26 then 'U'
		                                    when 27 then 'U'
		                                    when 40 then 'U'
		                                    when 41 then 'U'
		                                    when 42 then 'U'
		                                    when 51 then 'U'
		                                    when 52 then 'F'
		                                    when 53 then 'P'
		                                    when 54 then 'T'
		                                    when 55 then 'M'
		                                    when 56 then 'E'
		                                    when 58 then 'L'
		                                    when 59 then 'R'
		                                    when 60 then 'R'
		                                    when 61 then 'U'
		                                    when 62 then 'F'
		                                    when 63 then 'V'
		                                    when 64 then 'U'
		                                    when 65 then 'U'
		                                    when 66 then 'U'
		                                    when 76 then 'U'
		                                    when 101 then 'U'
		                                    when 102 then 'U'
		                                    when 103 then 'U'
		                                    when 104 then 'U'
		                                    when 105 then 'U'
		                                    when 106 then 'U'
		                                    when 107 then 'U'
		                                    when 108 then 'U'
		                                    when 109 then 'U'
		                                    when 110 then 'U'
		                                    when 111 then 'U'
		                                    when 112 then 'U'
		                                    when 113 then 'U'
		                                    when 114 then 'U'
		                                    when 115 then 'U'
		                                    when 116 then 'U'
		                                    when 201 then 'U'
		                                    when 202 then 'U'
		                                    when 203 then 'U'
		                                    when 204 then 'U'
		                                    when 205 then 'U'
		                                    when 206 then 'U'
		                                    when 208 then 'L'
		                                    when 301 then 'U'
		                                    when 302 then 'U'
		                                    when 303 then 'U'
		                                    when 304 then 'U'
		                                    when 305 then 'U'
		                                    when 306 then 'U'
		                                    when 307 then 'U'
		                                    when 308 then 'U'
		                                    when 309 then 'U'
		                                    when 310 then 'U'
		                                    when 311 then 'U'
		                                    when 312 then 'U'
		                                    when 313 then 'U'
		                                    when 314 then 'U'
		                                    when 315 then 'U'
		                                    when 316 then 'U'
		                                    when 317 then 'U'
		                                    when 318 then 'U'
		                                    when 319 then 'U'
		                                    when 415 then 'U'
		                                    when 416 then 'U'
		                                    when 420 then 'U'
		                                    when 421 then 'U'
		                                    when 510 then 'U'
		                                    when 511 then 'U'
		                                    when 901 then 'U'
		                                    when 902 then 'U'
		                                    when 903 then 'U'
		                                    when 904 then 'U'
		                                    when 905 then 'U'
		                                    when 906 then 'U'
		                                    when 997 then 'U'
		                                    when 999 then 'U'
	                                    end as Situacao
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
                                        ,1 as CodFilial
                                        ,funcionario.raccor as RacaCor
                                        ,case funcionario.tipcol
                                            when 1 then 1
                                            when 2 then 13
                                        end as CodCategoria,
                                    funcionario.numpis as NpisPasep,
                                    funcionario.dcdpis as DataCadastrosPIS,
                                    funcionario.confgt as ContaFGTS,
                                    funcionario.datsld as DataSaldoFGTS,
                                    funcionario.depsld as SaldoFGTS, 
                                    cast(funcionario.conban as varchar(15)) + cast(funcionario.digban as varchar(1)) as ContaCorrente,
                                    case when situacao.codsit in ('7', '659')
		                                      then (select datafa from vetorh.r038afa WHERE sitafa = situacao.codsit and numcad = funcionario.numcad and tipcol = funcionario.tipcol)
		                                      else null -- situacao.codsit
	                                    end as DataDemissao,
                                    funcionario.usu_terati
                                    from vetorh.r034fun as funcionario
                                    left join dbo.vw_totvs_chapafuncionario chapa on funcionario.numcad = chapa.numcad
												                                    and funcionario.tipcol = chapa.tipcol
												                                    and funcionario.taborg = chapa.taborg
                                    left join vetorh.r023nac as nacionalidade on nacionalidade.codnac=funcionario.codnac
                                    left join vetorh.r022gra as grauInstrucao on funcionario.GraIns=grauInstrucao.grains
                                    left join vetorh.r034cpl as ficha on ficha.numemp=funcionario.numemp
                                        and ficha.tipcol=funcionario.tipcol
                                        and ficha.numcad=funcionario.numcad
                                    left join vetorh.r074bai bairroEndereco on bairroEndereco.codcid=ficha.codcid
                                        and bairroEndereco.codbai=ficha.codbai
                                    left join vetorh.r074cid cidadeEndereco on cidadeEndereco.codcid=ficha.codcid
                                    left join vetorh.r074pai paisEndereco on paisEndereco.codpai=ficha.codpai
                                    left join vetorh.r074cid cidadeNatural on cidadeNatural.codcid=ficha.ccinas
                                    left join vetorh.r010sit situacao on situacao.codsit=funcionario.sitafa
                                    left join vetorh.r024car cargo on cargo.codcar=funcionario.codcar
                                    left join vetorh.r038hsi sind_fun on sind_fun.numemp=funcionario.numemp
                                        and sind_fun.tipcol=funcionario.tipcol
                                        and sind_fun.numcad=funcionario.numcad
                                        and sind_fun.datalt=funcionario.datsin
                                    left join vetorh.r006esc escala on escala.codesc=funcionario.codesc
                                    left join vetorh.r016hie secao on secao.numloc=funcionario.numloc
                                    where chapa.Chapa not in ('15000')
                                    and chapa.numcpf <> '0' order by funcionario.usu_terati asc";
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

            //validarDuplicados(funcionarios);

            FileHelperEngine engine = new FileHelperEngine(typeof(Funcionario), Encoding.Default);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, funcionarios);
        }

        private void validarDuplicados(List<Funcionario> funcionarios)
        {
            List<string> deleteFunc = new List<string>();

            foreach (var func in funcionarios.OrderBy(f => f.CPF))
            {
                if (funcionarios.Where(f => f.CPF == func.CPF).Count() > 1)
                {
                    _bgWorker.ReportProgress(0, String.Format("CPF {0} duplicado, Nome:{1}, Data Admissão:{2}", func.CPF, func.Nome, ((DateTime)func.DataAdmissao).ToString("dd/MM/yyyy")));

                    if (!deleteFunc.Contains(func.CPF))
                        deleteFunc.Add(func.CPF);
                }
            }

            foreach (var func in deleteFunc)
            {
                funcionarios.RemoveAll(f => f.CPF == func);
            }
        }

        private bool buscarFuncionarios(List<Funcionario> lFunc)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            _bgWorker.ReportProgress(0, "Executando consulta de Funcionários...");

            DbCommand command = database.GetSqlStringCommand(_queryFunc.Replace("{schemaName}", dbName));

            IDataReader drFunc = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            _bgWorker.ReportProgress(0, "Montando objetos...");

            double processedRecords = 0;

            while (drFunc.Read())
            {
                Funcionario func = new Funcionario();

                try
                {
                    processedRecords++;

                    func.Id = Convert.ToInt32(drFunc["Id"]);

                    var result = lFunc.Where(f => f.CPF == drFunc["CPF"].ToString().PadLeft(11, '0'));

                    if (result.Count() >= 1)
                    {
                        var value = result.First();

                        func.Nome = value.Nome;
                        func.Nascimento = value.Nascimento;
                        func.EstadoNatal = value.EstadoNatal;
                        func.Naturalidade = value.Naturalidade;
                    }
                    else
                    {
                        func.Nome = drFunc["Nome"].ToString().RemoveSpecialChars();
                        
                        if (drFunc["Nascimento"] != DBNull.Value)
                            func.Nascimento = Convert.ToDateTime(drFunc["Nascimento"]);

                        func.EstadoNatal = buscaEstado(drFunc["EstadoNatal"].ToString());

                        func.Naturalidade = drFunc["Naturalidade"].ToString().RemoveSpecialChars();
                    }
                    
                    //validarObrigatorios(drFunc);

                    func.Chapa = drFunc["Chapa"].ToString();
                    func.Chapa = func.Chapa.PadLeft(5, '0');
                    
                    func.Sexo = drFunc["Sexo"].ToString();
                    func.GrauInstrucao = BuscarGrauInstrucao(drFunc["GrauInstrucao"].ToString());
                    func.CPF = drFunc["CPF"].ToString().PadLeft(11, '0');
                    func.FichaRegistro = String.IsNullOrWhiteSpace(drFunc["FichaRegistro"].ToString().Replace("0", "")) ? Convert.ToInt32(func.Chapa) : Convert.ToInt32(drFunc["FichaRegistro"]);

                    if (drFunc["DataAdmissao"] != DBNull.Value)
                        func.DataAdmissao = Convert.ToDateTime(drFunc["DataAdmissao"]);
                    func.CodFuncao = drFunc["CodFuncao"].ToString();
                    func.TipoFuncionario = BuscarTipoFuncionario(drFunc["TipoFuncionario"].ToString());

                    func.CodSecao = drFunc["CodSecao"].ToString();

                    if (drFunc["TipoFuncionario"].ToString() == "Autônomo")
                    {
                        if (drFunc["usu_terati"].ToString() == "N")
                            func.Situacao = "D";
                        else
                            func.Situacao = "A";
                    }
                    else
                    {
                        func.Situacao = drFunc["Situacao"].ToString();
                    }

                    func.Apelido = drFunc["Apelido"].ToString().RemoveSpecialChars();
                    func.EstadoCivil = BuscarEstadoCivil(drFunc["EstadoCivil"].ToString());
                    func.Nacionalidade = BuscarNacionalidade(drFunc["Nacionalidade"].ToString());
                    func.Rua = drFunc["Rua"].ToString().RemoveSpecialChars().Replace(";", ".");
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

                    if (drFunc["CarteiraIdentidade"] != DBNull.Value && drFunc["CarteiraIdentidade"] != "0")
                    {
                        func.CarteiraIdentidade = drFunc["CarteiraIdentidade"].ToString();
                    }

                    func.UFCartIdentidade = buscaEstado(drFunc["UFCartIdentidade"].ToString());

                    func.OrgaoEmissorCartIdentidade = drFunc["OrgaoEmissorCartIdentidade"].ToString();

                    if (drFunc["DataDemissao"] != DBNull.Value)
                        func.DataDemissao = Convert.ToDateTime(drFunc["DataDemissao"]);

                    if (drFunc["EmissaoCartIdentidade"] != DBNull.Value)
                        func.EmissaoCartIdentidade = Convert.ToDateTime(drFunc["EmissaoCartIdentidade"]);

                    func.TituloEleitor = drFunc["TituloEleitor"].ToString();
                    func.ZonaVotacao = drFunc["ZonaVotacao"].ToString();
                    func.SecaoVotacao = drFunc["SecaoVotacao"].ToString();
                    func.CarteiraTrabalho = drFunc["CarteiraTrabalho"].ToString();
                    func.TipoCarteiraMotorista = drFunc["TipoCarteiraMotorista"].ToString();
                    func.CarteiraMotorista = drFunc["CarteiraMotorista"].ToString();

                    if (drFunc["VencCartMotorista"] != DBNull.Value)
                        func.VencCartMotorista = Convert.ToDateTime(drFunc["VencCartMotorista"]);

                    func.CertifReservista = drFunc["CertifReservista"].ToString();
                    func.CategMilitar = drFunc["CategMilitar"].ToString();
                    
                    if (drFunc["ChegadaAoBrasil"] != DBNull.Value)
                        func.ChegadaAoBrasil = Convert.ToDateTime(drFunc["ChegadaAoBrasil"].DefaultDbNull("1900-1-1"));
                    func.TipoRecebimento = BuscarTipoRecebimento(drFunc["TipoRecebimento"].ToString());
                    func.NumContaFGTS = drFunc["ContaFGTS"].ToString();
                    func.ContaPgto = drFunc["ContaCorrente"].ToString();

                    if (func.TipoRecebimento == "P")
                    {
                        func.Salario = 0;
                        func.UsaSalarioComposto = true;
                    }
                    else
                    {
                        func.Salario = Convert.ToDouble(drFunc["Salario"]);
                    }

                    func.CodSindicato = drFunc["CodSindicato"].ToString();
                    func.Jornada = drFunc["Jornada"].ToString();

                    // Foi definido que o código do horário será: 0001 
                    func.CodHorario = "0001";//drFunc["CodHorario"].ToString().PadLeft(4, '0');
                    func.SituacaoFGTS = BuscarSituacaoFGTS(drFunc["SituacaoFGTS"].ToString());
                    if (drFunc["DataOpcaoFGTS"] != DBNull.Value)
                        func.DataOpcaoFGTS = Convert.ToDateTime(drFunc["DataOpcaoFGTS"]);
                    func.TipoVisto = drFunc["TipoVisto"].ToString();
                    func.MembroSindical = ConverterBoolean(drFunc["MembroSindical"].ToString());
                    func.ContribuicaoSindical = drFunc["ContribuicaoSindical"].ToString();
                    func.TipoAdmissao = drFunc["TipoAdmissao"].ToString();

                    func.SituacaoRAIS = buscarSituacaoRAIS(drFunc["SituacaoRAIS"].ToString());
                    func.VinculoRAIS = String.IsNullOrWhiteSpace(func.SituacaoRAIS) ? String.Empty : buscarVinculoRAIS(drFunc["TipoFuncionario"].ToString());
                    func.CodFilial = Convert.ToInt32(drFunc["CodFilial"]);
                    func.CorRaca = buscarRacaCor(drFunc["RacaCor"].ToString());

                    if (drFunc["CodAgenciaPgto"] != "0" && drFunc["CodAgenciaPgto"] != null)
                    {
                        func.CodAgenciaPgto = drFunc["CodAgenciaPgto"].ToString();
                    }
                    if (drFunc["CodBancoPgto"] != "0" && drFunc["CodBancoPgto"] != null)
                    {
                        func.CodBancoPgto = drFunc["CodBancoPgto"].ToString();
                    }

                    func.CodCategoria = Convert.ToInt32(drFunc["CodCategoria"]);
                    func.SerieCarteiraTrabalho = drFunc["SerieCarteiraTrabalho"].ToString();
                    if (drFunc["EmissaoCarteiraTrabalho"] != DBNull.Value)
                        func.EmissaoCarteiraTrabalho = Convert.ToDateTime(drFunc["EmissaoCarteiraTrabalho"]);
                    func.PIS_PASEP = drFunc["NpisPasep"].ToString();
                    if (drFunc["EmissaoCarteiraTrabalho"] != DBNull.Value)
                        func.DataCadastrosPIS = Convert.ToDateTime(drFunc["DataCadastrosPIS"]);

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

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível importar o funcionário {0}, Id {1}. Motivo:{2}", func.Nome, func.Id, ex.StackTrace));
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

        private string buscaEstado(string p)
        {
            string codEstado = "";

            //if (String.IsNullOrWhiteSpace(from.Replace("0","")))
            //  throw new BusinessException("Funcionário não possui banco associado.");

            switch (p)
            {
                case "AC":
                    codEstado = p;
                    break;                

                case "AL":
                    codEstado = p;
                    break;

                case "AM":
                    codEstado = p;
                    break;

                case "AP":
                    codEstado = p;
                    break;

                case "BA":
                    codEstado = p;
                    break;

                case "CE":
                    codEstado = p;
                    break;

                case "DF":
                    codEstado = p;
                    break;

                case "ES":
                    codEstado = p;
                    break;

                case "GO":
                    codEstado = p;
                    break;

                case "MA":
                    codEstado = p;
                    break;

                case "MG":
                    codEstado = p;
                    break;

                case "MS":
                    codEstado = p;
                    break;

                case "MT":
                    codEstado = p;
                    break;

                case "PA":
                    codEstado = p;
                    break;

                case "PB":
                    codEstado = p;
                    break;

                case "PE":
                    codEstado = p;
                    break;

                case "PI":
                    codEstado = p;
                    break;

                case "PR":
                    codEstado = p;
                    break;

                case "RJ":
                    codEstado = p;
                    break;

                case "RN":
                    codEstado = p;
                    break;

                case "RO":
                    codEstado = p;
                    break;

                case "RR":
                    codEstado = p;
                    break;

                case "RS":
                    codEstado = p;
                    break;

                case "SC":
                    codEstado = p;
                    break;

                case "SE":
                    codEstado = p;
                    break;

                case "SP":
                    codEstado = p;
                    break;

                case "TO":
                    codEstado = p;
                    break;

                case "US":
                    codEstado = p;
                    break;                  

                default:
                    codEstado = "--";
                    break;
            }


            return codEstado;
        }

        private int buscarRacaCor(string from)
        {
            int corRaca = 0;

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

            //if (corRaca == 0)
            //    throw new BusinessException("Funcionário não possui cor/raça cadastrada.");

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
            //  throw new BusinessException("Funcionário não possui banco associado.");

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
            //if (String.IsNullOrWhiteSpace(from))
            //    throw new BusinessException("Funcionário não possui grau de instrucao associado.");

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
            //if (String.IsNullOrWhiteSpace(from))
            //    throw new BusinessException("Funcionário não possui Nacionalidade cadastrada.");


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
            //if (String.IsNullOrWhiteSpace(tipoContrato))
            //    throw new BusinessException("Funcionário não possui tipo de contrato associado.");

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
            //if (String.IsNullOrWhiteSpace(from))
            //    throw new BusinessException("Funcionário não possui situação RAIS associado.");

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
                    return "";

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
            /* if (String.IsNullOrWhiteSpace(drFunc["Chapa"].ToString()))
            {
                throw new BusinessException("Chapa do funcionário não cadastrada.");
            }

            if (String.IsNullOrWhiteSpace(drFunc["FichaRegistro"].ToString()))
            {
                //throw new BusinessException("Ficha registro do funcionário não cadastrada.");
            }

            if (String.IsNullOrWhiteSpace(drFunc["CarteiraIdentidade"].ToString()))
            {
                //throw new BusinessException("Carteira de identidade do funcionário não cadastrada.");
            }

            if ((String.IsNullOrWhiteSpace(drFunc["EstadoNatal"].ToString())) && (drFunc["TipoFuncionario"].ToString() != TipoFuncionario.Autonomo))
            {
                //throw new BusinessException("Quando o funcionário for do tipo Empregado ou Professor, o campo 'Estado Natal' deve estar preenchido.");
            }

            if ((String.IsNullOrWhiteSpace(drFunc["EstadoCivil"].ToString())) && (drFunc["TipoFuncionario"].ToString() != TipoFuncionario.Autonomo))
            {
                throw new BusinessException("Quando o funcionário for do tipo Empregado ou Professor, o campo 'Estado Civil' deve estar preenchido.");
            } */

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
                    return EstadoCivil.Solteiro;
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
