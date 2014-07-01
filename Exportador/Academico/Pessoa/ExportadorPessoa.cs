using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.Interface;
using Exportador.Helpers;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.DAO;

namespace Exportador.Academico.Pessoa
{
    public class ExportadorPessoa : IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private double processedRecords;
        private double totalRecords;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private bool _debugMode;
        private List<Profissao> profissoes;
        private List<Ocupacao> ocupacoes;
        private List<Nacionalidade> nacionalidades;
        private List<Estado> estados;
        private List<EnderecoUsuario> enderecos = new List<EnderecoUsuario>();
        private Dictionary<int, string> estadosCivis = new Dictionary<int, string>();
        private Dictionary<int, string> corRaca = new Dictionary<int, string>();
        private Dictionary<int, string> naturalidades = new Dictionary<int, string>();

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
        public ExportadorPessoa()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPessoa(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPessoa(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryEnderecos = @"select
                                                ID_USUARIO,
                                                LOGRADOURO,
                                                NUMERO,
                                                COMPLEMENTO,
                                                BAIRRO,
                                                UF,
                                                LOCALIDADE,
                                                PAIS
                                            from usuario_endereco";

        private string _queryRacaCor = "SELECT ID,NOME FROM RACA_COR";

        private string _queryNaturalidades = "SELECT ID,NOME FROM CEP_LOCALIDADES";

        private static string _queryPessoas = @"SELECT
                                                    U.ID as CODIGO,
                                                    U.NOME as NOME,
                                                    U.NOME as APELIDO,
                                                    U.DATA_NASCIMENTO as DTNASCIMENTO,
                                                    EC.NOME as ESTADOCIVIL,
                                                    U.SEXO as SEXO,
                                                    COALESCE(CL.NOME,'Indefinido') as NATURALIDADE,
                                                    COALESCE(CU.SIGLA,'--') as ESTADONATAL,
                                                    NC.NOME as NACIONALIDADE,
                                                    U.CPF as CPF,
                                                    U.FONE_COM as TELEFONE1,
                                                    U.CELULAR as TELEFONE2,
                                                    U.FONE_RES as TELEFONE3,
                                                    U.EMAIL as EMAIL,
                                                    U.RG as CARTIDENTIDADE,
                                                    OE.NOME as ORGEMISSORIDENT,
                                                    U.DATA_EXPEDICAO_RG as DTEMISSAOIDENT,
                                                    U.TITULO as TITULOELEITOR,
                                                    U.ZONA as ZONATITELEITOR,
                                                    U.SECAO as SECAOTITELEITOR,
                                                    U.DOCUMENTO_MILITAR as CERTIFRESERV,
                                                    U.DATA_EXPEDICAO_MIL as DTEXPCML,
                                                    OE2.NOME as EXPED,
                                                    NC.NOME as PAISORIGEM,
                                                    RACA_COR.NOME as CORRACA,
                                                    IIF(A.ID_NECESSIDADE_ESPECIAL = 4,1,0) as DEFICIENTEFISICO,
                                                    IIF(A.ID_NECESSIDADE_ESPECIAL = 2,1,0) as DEFICIENTEAUDITIVO,
                                                    IIF(A.ID_NECESSIDADE_ESPECIAL = 3,1,0) as DEFICIENTEVISUAL,
                                                    IIF(A.ID_NECESSIDADE_ESPECIAL = 5,1,0) as DEFICIENTEMENTAL,
                                                    P.NOME as PROFISSAO,
                                                    P.NOME as OCUPACAO,
                                                    U.ALUNO as ALUNO,
                                                    IIF(PROF.ID_USUARIO IS NOT NULL AND PROF.ID_SITUACAO_PROFESSOR <> 3, 1, 0) as PROFESSOR,
                                                    IIF(F.ID_USUARIO IS NOT NULL AND F.ID_SITUACAO_FUNCIONARIO <> 3, 1, 0) as FUNCIONARIO,
                                                    IIF(F.ID_USUARIO IS NOT NULL AND F.ID_SITUACAO_FUNCIONARIO = 3, 1, 0) as EXFUNCIONARIO
                                                    FROM
                                                USUARIO U
                                                LEFT JOIN ALUNO A ON (U.ID = A.ID_USUARIO)
                                                LEFT JOIN PROFESSOR PROF ON (U.ID = PROF.ID_USUARIO)
                                                LEFT JOIN FUNCIONARIO F ON (U.ID = F.ID_USUARIO)

                                                LEFT JOIN ESTADO_CIVIL EC ON (U.ID_ESTADO_CIVIL = EC.ID)
                                                LEFT JOIN CEP_LOCALIDADES CL ON (U.ID_NATURALIDADE = CL.ID)
                                                LEFT JOIN CEP_UFS CU ON (CL.ID_CEP_UF = CU.ID)
                                                LEFT JOIN NACIONALIDADE NC ON (U.ID_NACIONALIDADE = NC.ID)
                                                LEFT JOIN ORGAO_EXPEDIDOR OE ON (U.ID_ORGAO_EXPEDIDOR_RG = OE.ID)
                                                LEFT JOIN ORGAO_EXPEDIDOR OE2 ON (U.ID_ORGAO_EXPEDIDOR_MIL = OE2.ID)
                                                LEFT JOIN RACA_COR ON A.ID_RACA_COR=RACA_COR.ID
                                                LEFT JOIN PROFISSAO P ON (U.ID_PROFISSAO = P.ID)
                                                LEFT JOIN EMPRESA E ON (U.ID_EMPRESA = E.ID)

                                                    WHERE U.ID NOT IN (0,27144)
                                                        AND U.NOME IS NOT NULL
                                                        AND (
                                                            A.ID_USUARIO IS NOT NULL
                                                                OR
                                                            PROF.ID_USUARIO IS NOT NULL
                                                            )
                                                    ORDER BY U.ID";


        private static string _queryCountPessoas = @"SELECT COUNT(*) AS COUNTER
                                                FROM USUARIO U
                                                LEFT JOIN ALUNO A ON (U.ID = A.ID_USUARIO)
                                                LEFT JOIN PROFESSOR PROF ON (U.ID = PROF.ID_USUARIO) 
                                                WHERE U.ID NOT IN (0,27144)
                                                AND U.NOME IS NOT NULL
                                                AND (
                                                    A.ID_USUARIO IS NOT NULL
                                                        OR
                                                    PROF.ID_USUARIO IS NOT NULL
                                                    )";

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

            _bgWorker.ReportProgress(0, "Buscando ocupações...");

            OcupacaoDAO ocupDAO = new OcupacaoDAO();
            ocupacoes = ocupDAO.buscarTodas();

            _bgWorker.ReportProgress(0, "Buscando profissões...");

            ProfissaoDAO profDAO = new ProfissaoDAO();
            profissoes = profDAO.buscarTodas();

            _bgWorker.ReportProgress(0, "Buscando nacionalidades...");

            NacionalidadeDAO nacDAO = new NacionalidadeDAO();
            nacionalidades = nacDAO.buscarTodas();

            _bgWorker.ReportProgress(0, "Buscando estados...");

            EstadoDAO estDAO = new EstadoDAO();
            estados = estDAO.buscarTodas();




            _bgWorker.ReportProgress(0, "Buscando pessoas...");

            List<Pessoa> pessoas = new List<Pessoa>();

            error = buscarPessoas(pessoas);



            _bgWorker.ReportProgress(0, "Iniciando geração de arquivo...");

            FileHelperEngine engine = new FileHelperEngine(typeof(Pessoa), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, pessoas);

        }

        private bool buscarPessoas(List<Pessoa> pessoas)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            _bgWorker.ReportProgress(0, "Buscando endereços...");

            using (DbCommand cmd = database.GetSqlStringCommand(_queryEnderecos))
            {
                using (IDataReader drEnderecos = database.ExecuteReader(cmd))
                {
                    while (drEnderecos.Read())
                    {
                        EnderecoUsuario endUsu = new EnderecoUsuario();

                        endUsu.UsuarioId = (int)drEnderecos.GetNullableInt32("ID_USUARIO");
                        endUsu.Logradouro = drEnderecos.GetString("LOGRADOURO");
                        endUsu.Numero = drEnderecos.GetString("NUMERO");
                        endUsu.Complemento = drEnderecos.GetString("COMPLEMENTO");
                        endUsu.Bairro = drEnderecos.GetString("BAIRRO");
                        endUsu.UF = drEnderecos.GetString("UF");
                        endUsu.Localidade = drEnderecos.GetString("BAIRRO");
                        endUsu.Pais = drEnderecos.GetString("PAIS");

                        enderecos.Add(endUsu);
                    }
                }
            }

            _bgWorker.ReportProgress(0, "Buscando cores/raças...");

            using (DbCommand cmd = database.GetSqlStringCommand(_queryRacaCor))
            {
                using (IDataReader drCorRaca = database.ExecuteReader(cmd))
                {
                    while (drCorRaca.Read())
                    {
                        int idCorRaca = (int)drCorRaca.GetNullableInt32("ID");
                        string nomeCorRaca = drCorRaca.GetString("NOME");

                        corRaca.Add(idCorRaca, nomeCorRaca);
                    }
                }
            }

            _bgWorker.ReportProgress(0, "Buscando naturalidades...");

            using (DbCommand cmd = database.GetSqlStringCommand(_queryNaturalidades))
            {
                using (IDataReader drNaturalidade = database.ExecuteReader(cmd))
                {
                    while (drNaturalidade.Read())
                    {
                        int idCorRaca = (int)drNaturalidade.GetNullableInt32("ID");
                        string nomeCorRaca = drNaturalidade.GetString("NOME");

                        naturalidades.Add(idCorRaca, nomeCorRaca);
                    }
                }
            }


            _bgWorker.ReportProgress(0, "Gerando contadores...");

            totalRecords = getCount();



            using (DbCommand command = database.GetSqlStringCommand(_queryPessoas))
            {
                _bgWorker.ReportProgress(0, "Efetuando busca de pessoas...");

                using (IDataReader drPessoas = database.ExecuteReader(command))
                {
                    processedRecords = 0;

                    while (drPessoas.Read())
                    {
                        Pessoa pessoa = new Pessoa();

                        try
                        {
                            processedRecords++;

                            pessoa = mapearPessoa(drPessoas);

                            pessoas.Add(pessoa);

                            if ((processedRecords % 100 == 0) && (_bgWorker != null))
                                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));

                        }
                        catch (Exception ex)
                        {
                            error = true;

                            if (_bgWorker != null)
                                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a pessoa: Código {0},Motivo:{1}.", drPessoas["CODIGO"], ex.Message));
                        }
                        if (_bgWorker != null)
                            _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }

                }
            }


            return error;

        }

        private double getCount()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            object count;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountPessoas))
            {
                count = database.ExecuteScalar(command);
            }

            return Convert.ToDouble(count);
        }

        private Pessoa mapearPessoa(IDataRecord record)
        {
            Pessoa pessoa = new Pessoa();

            pessoa.Codigo = (DBNull.Value == record["CODIGO"]) ? 0 : (int)record["CODIGO"];
            pessoa.Nome = (DBNull.Value == record["NOME"]) ? String.Empty : (string)record["NOME"];
            pessoa.Apelido = (DBNull.Value == record["APELIDO"]) ? String.Empty : (string)record["APELIDO"];
            pessoa.DtNascimento = record.GetNullableDateTime("DTNASCIMENTO");
            pessoa.Sexo = (DBNull.Value == record["SEXO"]) ? String.Empty : (string)record["SEXO"];
            pessoa.Naturalidade = (DBNull.Value == record["NATURALIDADE"]) ? String.Empty : (string)record["NATURALIDADE"];
            pessoa.CPF = (DBNull.Value == record["CPF"]) ? String.Empty : (string)record["CPF"];
            pessoa.Telefone1 = (DBNull.Value == record["TELEFONE1"]) ? String.Empty : (string)record["TELEFONE1"];
            pessoa.Telefone2 = (DBNull.Value == record["TELEFONE2"]) ? String.Empty : (string)record["TELEFONE2"];
            pessoa.Telefone3 = (DBNull.Value == record["TELEFONE3"]) ? String.Empty : (string)record["TELEFONE3"];
            pessoa.EMail = (DBNull.Value == record["EMAIL"]) ? String.Empty : (string)record["EMAIL"];
            pessoa.CartIdentidade = (DBNull.Value == record["CARTIDENTIDADE"]) ? String.Empty : (string)record["CARTIDENTIDADE"];
            pessoa.OrgEmissorIdent = ((DBNull.Value == record["ORGEMISSORIDENT"]) || (((string)record["ORGEMISSORIDENT"]) == "Indefinido")) ? String.Empty : ((string)record["ORGEMISSORIDENT"]);
            pessoa.DtEmissaoIdent = record.GetNullableDateTime("DTEMISSAOIDENT");
            pessoa.TituloEleitor = (DBNull.Value == record["TITULOELEITOR"]) ? String.Empty : (string)record["TITULOELEITOR"];
            pessoa.ZonaTitEleitor = (DBNull.Value == record["ZONATITELEITOR"]) ? String.Empty : (string)record["ZONATITELEITOR"];
            pessoa.SecaoTitEleitor = (DBNull.Value == record["SECAOTITELEITOR"]) ? String.Empty : (string)record["SECAOTITELEITOR"];
            pessoa.SecaoTitEleitor = (DBNull.Value == record["SECAOTITELEITOR"]) ? String.Empty : (string)record["SECAOTITELEITOR"];
            pessoa.CertifReserv = (DBNull.Value == record["CERTIFRESERV"]) ? String.Empty : (string)record["CERTIFRESERV"];
            pessoa.DtExpCml = record.GetNullableDateTime("DTEXPCML");
            pessoa.Exped = ((DBNull.Value == record["EXPED"]) || (((string)record["EXPED"]) == "Indefinido")) ? String.Empty : (string)record["EXPED"];
            pessoa.PaisOrigem = (DBNull.Value == record["PAISORIGEM"]) ? String.Empty : (string)record["PAISORIGEM"];
            pessoa.CorRaca = buscarCorRaca(record["CORRACA"]);
            pessoa.DeficienteFisico = DBHelper.GetNullableBoolean(record["DEFICIENTEFISICO"]);
            pessoa.DeficienteAuditivo = DBHelper.GetNullableBoolean(record["DEFICIENTEAUDITIVO"]);
            pessoa.DeficienteVisual = DBHelper.GetNullableBoolean(record["DEFICIENTEVISUAL"]);
            pessoa.DeficienteMental = DBHelper.GetNullableBoolean(record["DEFICIENTEMENTAL"]);
            pessoa.Aluno = DBHelper.GetNullableBoolean(record["ALUNO"]);
            pessoa.Professor = DBHelper.GetNullableBoolean(record["PROFESSOR"]);
            pessoa.Funcionario = DBHelper.GetNullableBoolean(record["FUNCIONARIO"]);
            pessoa.ExFuncionario = DBHelper.GetNullableBoolean(record["EXFUNCIONARIO"]);

            pessoa.Profissao = buscarProfissao(record);
            pessoa.Ocupacao = buscarOcupacao(record);
            pessoa.EstadoCivil = buscarEstadoCivil(record["ESTADOCIVIL"]);
            pessoa.Nacionalidade = buscarNacionalidade(record);
            pessoa.EstadoNatal = buscarEstadoNatal(record);
            buscarEndereco(pessoa);

            pessoa.NIT = true;
            pessoa.UsuarioBiblios = false;
            pessoa.Candidato = false;


            validarObrigatorios(pessoa);

            return pessoa;
        }

        private void buscarEndereco(Pessoa pessoa)
        {

            EnderecoUsuario endUsu = enderecos.Where(end => end.UsuarioId == pessoa.Codigo).FirstOrDefault();

            if (endUsu != null)
            {
                pessoa.Rua = endUsu.Logradouro;
                pessoa.Numero = endUsu.Numero;
                pessoa.Complemento = endUsu.Complemento;
                pessoa.Bairro = endUsu.Bairro;
                pessoa.Estado = endUsu.Estado;
                pessoa.Cidade = endUsu.Localidade;
                pessoa.Pais = endUsu.Pais;


            }
            else
            {
                _bgWorker.ReportProgress(0, String.Format("Pessoa id {0} não possui endereço.", pessoa.Codigo));
            }
        }

        private string buscarEstadoNatal(IDataRecord record)
        {
            string estadoNatal = (DBNull.Value == record["ESTADONATAL"]) ? String.Empty : ((string)record["ESTADONATAL"]).ToUpper();
            string paisNatal = (DBNull.Value == record["NATURALIDADE"]) ? String.Empty : ((string)record["NATURALIDADE"]).ToUpper();

            var estado = estados.FirstOrDefault(x => x.Codigo.RemoveSpecialChars().ToUpper().Equals(estadoNatal));

            if (estado == null)
                throw new BusinessException(String.Format("Estado Natal não cadastrado. Estado:{0}, País:{1}", estadoNatal, paisNatal));

            return estado.Codigo;
        }

        private void validarObrigatorios(Pessoa pessoa)
        {
            if (string.IsNullOrEmpty(pessoa.Nome))
                throw new BusinessException("Nome da pessoa é obrigatório.");

            if (pessoa.DtNascimento == null)
                throw new BusinessException("Data de nascimento é obrigatório.");

            if (string.IsNullOrEmpty(pessoa.Sexo))
                throw new BusinessException("Sexo da pessoa é obrigatório.");

            if (string.IsNullOrEmpty(pessoa.Naturalidade))
                throw new BusinessException("Naturalidade da pessoa é obrigatório.");

            if (string.IsNullOrEmpty(pessoa.EstadoNatal))
                throw new BusinessException("Estado Natal da pessoa é obrigatório.");

            if (string.IsNullOrEmpty(pessoa.Nacionalidade))
                throw new BusinessException("Nacionalidade da pessoa é obrigatório.");

            if (pessoa.DeficienteFisico == null)
                throw new BusinessException("Campo Deficiente Físico é obrigatório.");

            if (pessoa.DeficienteAuditivo == null)
                throw new BusinessException("Campo Deficiente Auditivo é obrigatório.");

            if (pessoa.DeficienteFala == null)
                throw new BusinessException("Campo Deficiente Fala é obrigatório.");

            if (pessoa.DeficienteVisual == null)
                throw new BusinessException("Campo Deficiente Visual é obrigatório.");

            if (pessoa.DeficienteMental == null)
                throw new BusinessException("Campo Deficiente Mental é obrigatório.");

            if (pessoa.Aluno == null)
                throw new BusinessException("Campo Aluno é obrigatório.");

            if (pessoa.Professor == null)
                throw new BusinessException("Campo Professor é obrigatório.");

            if (pessoa.UsuarioBiblios == null)
                throw new BusinessException("Campo UsuarioBiblios é obrigatório.");

            if (pessoa.Funcionario == null)
                throw new BusinessException("Campo Funcionario é obrigatório.");

            if (pessoa.ExFuncionario == null)
                throw new BusinessException("Campo Ex-Funcionario é obrigatório.");

            if (pessoa.Candidato == null)
                throw new BusinessException("Campo Candidato é obrigatório.");

        }

        private string buscarNacionalidade(IDataRecord record)
        {
            string descricao = (DBNull.Value == record["NACIONALIDADE"]) ? String.Empty : ((string)record["NACIONALIDADE"]).RemoveSpecialChars().ToUpper();

            var nacionalidade = nacionalidades.FirstOrDefault(x => x.Descricao.RemoveSpecialChars().ToUpper().Equals(descricao));

            if (nacionalidade == null)
            {
                nacionalidade = nacionalidades.FirstOrDefault(x => x.Descricao.RemoveSpecialChars().ToUpper().Equals("OUTROS"));
            }

            return nacionalidade.Codigo;
        }

        private String buscarOcupacao(IDataRecord record)
        {
            string ocupacaoAluno = record["OCUPACAO"].ToString().RemoveSpecialChars();

            var codOcupacao = ocupacoes.FirstOrDefault(x => x.Descricao.RemoveSpecialChars().Equals(ocupacaoAluno));

            if (codOcupacao != null)
            {
                return codOcupacao.CodCliente;
            }

            else
            {
                return String.Empty;
            }

        }

        private int? buscarProfissao(IDataRecord record)
        {
            string profissaoAluno = record["PROFISSAO"].ToString().RemoveSpecialChars();

            var codProfissao = profissoes.FirstOrDefault(x => x.Descricao.RemoveSpecialChars().Equals(profissaoAluno));

            if (codProfissao != null)
            {
                return codProfissao.CodCliente;
            }
            else
            {
                return null;
            }
        }

        private string buscarCodOcupacao(object descOcupacao)
        {
            OcupacaoDAO oDAO = new OcupacaoDAO();

            return oDAO.buscarCodOcupacao(descOcupacao.ToString().RemoveSpecialChars());
        }

        private Int32? buscarCodProfissao(object descProfissao)
        {
            ProfissaoDAO pDAO = new ProfissaoDAO();

            return pDAO.buscarCodProfissao(descProfissao.ToString().RemoveSpecialChars());
        }

        /// <summary>
        /// Retorna o código da cor/raça com base no texto informado.
        /// </summary>
        /// <param name="corRaca"></param>
        /// <returns>(0=Indígena:2=Branca:4=Preta:6=Amarela:8=Parda)</returns>
        private int? buscarCorRaca(object corRaca)
        {
            switch (corRaca.ToString())
            {
                case "Amarela":
                    return 6;
                case "Branca":
                    return 2;
                case "Indígena":
                    return 0;
                case "Parda/mestiça":
                    return 8;
                case "Preta/negra":
                    return 4;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Retorna o estado civil da pessoa, com base nos possíveis valores do sistema.
        /// (C=Casado:D=Desquitado:I=Divorciado:O=Outros:S=Solteiro:V=Viúvo)
        /// </summary>
        /// <param name="estadoCivil"></param>
        /// <returns></returns>
        private string buscarEstadoCivil(object estadoCivil)
        {
            if ((estadoCivil == null) || (estadoCivil == DBNull.Value))
                return "O";

            switch ((string)estadoCivil)
            {
                case "Casado":
                    return "C";

                case "Separado":
                case "Divorciado":
                    return "I";

                case "Solteiro":
                    return "S";

                case "Viúvo":
                    return "V";

                default:
                    return "O";
            }

        }

    }


    public class EnderecoUsuario
    {
        public int UsuarioId;

        public string Logradouro;
        public string Rua;
        public string Numero;
        public string Complemento;
        public string Bairro;
        public string Estado;
        public string Localidade;
        public string UF;
        public string Pais;
    }
}
