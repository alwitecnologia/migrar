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
using Exportador.Academico.Curso;
using Exportador.DAO;
using Exportador.Academico.Pessoa;
using Exportador.BackOffice.ClienteFornecedor;


namespace Exportador.Academico.Aluno
{
    public class ExportadorSAluno : IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private double _progress;
        private bool _debugMode;
        private List<Estado> estados;
        private List<ClienteFornecedor> clientes;
        private CursoDAO _cursoDAO = new CursoDAO();

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
        public ExportadorSAluno()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorSAluno(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorSAluno(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private static string _queryAlunos = @"SELECT

                                                    NC.NOME AS NIVELCURSO,
                                                    CURSO.NOME AS NOMECURSO,

                                                    u_aluno.id as IDALUNO,
                                                    U_ALUNO.NOME as NOME,
                                                    U_ALUNO.DATA_NASCIMENTO as DTNASCIMENTO,
                                                    U_ALUNO.CPF as CPF,
                                                    U_ALUNO.RG as CARTIDENTIDADE,
                                                    OE.NOME as UFCARTIDENT,
                                                    U_ALUNO.MATRICULA AS RA,
                                                    CL.NOME AS NATURALIDADE,
                                                    REPLACE(CU.SIGLA,'--','') AS ESTADONATAL,
                                                    SIT_ALUNO.NOME SIT_ALUNO,

                                                    U_PAI.NOME AS NOMEPAI,
                                                    U_PAI.DATA_NASCIMENTO AS DTNASCIMENTOPAI,
                                                    U_PAI.CPF AS CPFPAI,
                                                    U_PAI.RG AS RGPAI,
                                                    IIF(U_PAI.DATA_FALECIMENTO IS NULL,1,0) AS PAIVIVO,
                                                    COALESCE(CLPAI.NOME,'Indefinido') AS NATURALIDADEPAI,
                                                    REPLACE(CUPAI.SIGLA,'--','') AS ESTADONATALPAI,

                                                    U_MAE.NOME AS NOMEMAE,
                                                    U_MAE.DATA_NASCIMENTO AS DTNASCIMENTOMAE,
                                                    U_MAE.CPF AS CPFMAE,
                                                    U_MAE.RG AS RGMAE,
                                                    IIF(U_MAE.DATA_FALECIMENTO IS NULL,1,0) AS MAEVIVA,
                                                    COALESCE(CLMAE.NOME,'Indefinido') AS NATURALIDADEMAE,
                                                    REPLACE(CUMAE.SIGLA,'--','') AS ESTADONATALMAE

                                                FROM ALUNO
                                                INNER JOIN USUARIO U_ALUNO ON U_ALUNO.ID=ALUNO.ID_USUARIO
                                                LEFT JOIN ORGAO_EXPEDIDOR OE ON (U_ALUNO.ID_ORGAO_EXPEDIDOR_RG = OE.ID)
                                                LEFT JOIN CEP_LOCALIDADES CL ON (U_ALUNO.ID_NATURALIDADE = CL.ID)
                                                LEFT JOIN CEP_UFS CU ON (CL.ID_CEP_UF = CU.ID)

                                                LEFT JOIN USUARIO U_PAI ON (U_PAI.ID = U_ALUNO.ID_PAI)
                                                LEFT JOIN CEP_LOCALIDADES CLPAI ON (U_PAI.ID_NATURALIDADE = CLPAI.ID)
                                                LEFT JOIN CEP_UFS CUPAI ON (CLPAI.ID_CEP_UF = CUPAI.ID)

                                                LEFT JOIN USUARIO U_MAE ON (U_MAE.ID = U_ALUNO.ID_MAE)
                                                LEFT JOIN CEP_LOCALIDADES CLMAE ON (U_MAE.ID_NATURALIDADE = CLMAE.ID)
                                                LEFT JOIN CEP_UFS CUMAE ON (CLMAE.ID_CEP_UF = CUMAE.ID)

                                                INNER JOIN (
                                                    SELECT ID_ALUNO,MAX(ID) MAX_ID
                                                    FROM MATRICULA_CURSO
                                                    GROUP by ID_ALUNO
                                                ) AS MAX_MC ON MAX_MC.ID_ALUNO=U_ALUNO.ID
                                                INNER JOIN MATRICULA_CURSO MC ON MC.ID_ALUNO=U_ALUNO.ID
                                                    AND MC.ID=MAX_MC.MAX_ID
                                                INNER JOIN CURSO ON CURSO.ID=MC.ID_CURSO
                                                INNER JOIN SITUACAO_MATRICULA_CURSO SIT_ALUNO ON SIT_ALUNO.ID=MC.ID_SITUACAO_MATRICULA_CURSO
                                                INNER JOIN NIVEL_CURSO NC ON NC.ID=CURSO.ID_NIVEL_CURSO
                                                WHERE 
                                                    U_ALUNO.ID NOT IN (0,27144)
                                                    AND U_ALUNO.NOME IS NOT NULL
                                                    AND ALUNO.ID_USUARIO>0
                                                    AND U_ALUNO.ALUNO=1
                                                ORDER BY U_ALUNO.NOME";

        private static string _queryCountAlunos = @"SELECT COUNT(*)
                                            FROM (" + _queryAlunos + ") AS COUNTER";


        private string _queryBuscarCurso = @"SELECT FIRST 1 MC.ID_CURSO
                                            FROM MATRICULA_CURSO MC
                                            WHERE MC.ID_ALUNO=@ID_ALUNO
                                            ORDER BY MC.ID DESC";

        private string _queryBuscarTipoAluno = @"SELECT FIRST 1 SMC.NOME
                                                FROM MATRICULA_CURSO MC
                                                INNER JOIN SITUACAO_MATRICULA_CURSO SMC ON SMC.ID=MC.ID_SITUACAO_MATRICULA_CURSO
                                                WHERE MC.ID_ALUNO=@ID_ALUNO
                                                ORDER BY MC.ID DESC";

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
            
            EstadoDAO estDAO = new EstadoDAO();
            estados = estDAO.buscarTodas();

            ClienteDAO cliDAO = new ClienteDAO();
            clientes = cliDAO.buscarTodosRM();

            List<Aluno> alunos = new List<Aluno>();

            error = buscarAlunos(alunos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Aluno), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, alunos);

        }

        private bool buscarAlunos(List<Aluno> alunos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand command = database.GetSqlStringCommand(_queryAlunos);

            if(_debugMode)
                _bgWorker.ReportProgress(0, "Gerando contadores para exportação de alunos...");

            double totalRecords = getCount();

            if (_debugMode)
                _bgWorker.ReportProgress(0, "Efetuando busca de alunos...");

            double processedRecords = 0;

            IDataReader drAlunos = database.ExecuteReader(command);

            if (_debugMode)
                _bgWorker.ReportProgress(0, "Iniciando geração de arquivo...");

            while (drAlunos.Read())
            {
                Aluno aluno = new Aluno();

                try
                {
                    processedRecords++;

                    _progress = processedRecords / totalRecords * 100;

                    aluno.IdAluno = Convert.ToInt32(drAlunos["IDALUNO"]);

                    if (_debugMode)
                        _bgWorker.ReportProgress(Convert.ToInt32(_progress), String.Format("Buscando informações aluno id:{0}...",aluno.IdAluno.ToString()));

                    //Dados aluno...
                    aluno.Nome = drAlunos["NOME"].ToString().RemoveSpecialChars();
                    aluno.Sobrenome = buscarSobrenome(drAlunos["NOME"].ToString()).RemoveSpecialChars();
                    aluno.DtNascimento = drAlunos.GetNullableDateTime("DTNASCIMENTO");                
                    aluno.CPF = drAlunos["CPF"].ToString();
                    aluno.CartIdentidade = drAlunos["CARTIDENTIDADE"].ToString();
                    aluno.UFCartIdent = drAlunos["UFCARTIDENT"].ToString();
                    aluno.RA = drAlunos["RA"].ToString();
                    aluno.Naturalidade = drAlunos["NATURALIDADE"].ToString();
                    aluno.CodTipoCurso = _cursoDAO.buscarTipoCurso((string)drAlunos["NIVELCURSO"], (string)drAlunos["NOMECURSO"]);
                    aluno.TipoAluno = drAlunos["SIT_ALUNO"].ToString().RemoveSpecialChars();
                    aluno.EstadoNatal = buscarEstadoNatal((string)drAlunos["ESTADONATAL"].DefaultDbNull(String.Empty), (string)drAlunos["NATURALIDADE"].DefaultDbNull(String.Empty));

                    aluno.CertUF = drAlunos["ESTADONATAL"].ToString();

                    //Dados pai...
                    aluno.NomePai = drAlunos["NOMEPAI"].ToString().RemoveSpecialChars();
                    aluno.DtNascimentoPai = drAlunos.GetNullableDateTime("DTNASCIMENTOPAI");
                    aluno.CpfPai = drAlunos["CPFPAI"].ToString();
                    aluno.RgPai = drAlunos["RGPAI"].ToString();
                    aluno.PaiVivo = Convert.ToBoolean(drAlunos["PAIVIVO"]);
                    aluno.NaturalidadePai = drAlunos["NATURALIDADEPAI"].ToString();
                    aluno.EstadoNatalPai = buscarEstadoNatal((string)drAlunos["ESTADONATALPAI"].DefaultDbNull(String.Empty), (string)drAlunos["NATURALIDADEPAI"].DefaultDbNull(String.Empty));

                    //Dados mãe...
                    aluno.NomeMae = drAlunos["NOMEMAE"].ToString().RemoveSpecialChars();
                    aluno.DtNascimentoMae = drAlunos.GetNullableDateTime("DTNASCIMENTOMAE");
                    aluno.CpfMae = drAlunos["CPFMAE"].ToString();
                    aluno.RgMae = drAlunos["RGMAE"].ToString();
                    aluno.MaeViva = Convert.ToBoolean(drAlunos["MAEVIVA"]);
                    aluno.NaturalidadeMae = drAlunos["NATURALIDADEMAE"].ToString();
                    aluno.EstadoNatalMae = buscarEstadoNatal((string)drAlunos["ESTADONATALMAE"].DefaultDbNull(String.Empty), (string)drAlunos["NATURALIDADEMAE"].DefaultDbNull(String.Empty));//drAlunos["ESTADONATALMAE"].ToString();

                    //Dados financeiro...
                    aluno.CodCfo = buscarCodFinanceiro(drAlunos);

                    //Dados fixos...
                    aluno.TipoCertidao = "CN";
                    aluno.CodColigada = 1;
                    aluno.CodColCFO = 1;
                    aluno.CodParentCfo = "9";
                    aluno.CodParentRaca = "9";
                    aluno.NomeRespAcad = aluno.Nome;
                    aluno.CpfRespAcad = aluno.CPF;
                    aluno.RgRespAcad = aluno.CartIdentidade;
                    aluno.NaturalidadeAcad = aluno.Naturalidade;
                    aluno.EstadoNatalAcad = aluno.EstadoNatal;

                    validarCamposObrigatorios(aluno);

                    alunos.Add(aluno);

                }
                catch (BusinessException ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(_progress), String.Format("Não foi possível exportar o aluno: IdAluno {0},Motivo:{1}", aluno.IdAluno, ex.Message));
                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(_progress), String.Format("Não foi possível exportar o aluno: IdAluno {0},Motivo:{1}.\n {2}", aluno.IdAluno, ex.Message, ex.StackTrace));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(_progress));
            }

            return error;

        }

        private string buscarCodFinanceiro(IDataRecord drAlunos)
        {
            string cpfResponsavel = (DBNull.Value == drAlunos["CPF"]) ? String.Empty : drAlunos["CPF"].ToString().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty).PadLeft(11, '0');
            string matriculaResponsavel = (DBNull.Value == drAlunos["RA"]) ? String.Empty : drAlunos["RA"].ToString();
            string nomeResponsavel = (DBNull.Value == drAlunos["NOME"]) ? String.Empty : drAlunos["NOME"].ToString();

            var cliente = clientes.FirstOrDefault(x => x.CNPJCPF.RemoveSpecialChars().ToUpper().Equals(cpfResponsavel.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty).PadLeft(11, '0')));

            if (cliente == null && (!String.IsNullOrEmpty(matriculaResponsavel)))
                cliente = clientes.FirstOrDefault(x => x.CodigoAcademico.RemoveSpecialChars().ToUpper().Equals(matriculaResponsavel));

            if (cliente == null)
                throw new BusinessException(String.Format("Não encontrado no sistema financeiro. Nome:{0}; CPF:{1}; Matricula SICA:{2}",nomeResponsavel,cpfResponsavel,matriculaResponsavel));

            return cliente.Codigo;
        }

        private double getCount(){
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            object count;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountAlunos))
            {
                count = database.ExecuteScalar(command);
            }

            return Convert.ToDouble(count);
        }

        private void validarCamposObrigatorios(Aluno aluno)
        {
            if (String.IsNullOrWhiteSpace(aluno.EstadoNatal))
                throw new BusinessException("Aluno não possui estado natal preenchido.");

            if (String.IsNullOrWhiteSpace(aluno.RA))
                throw new BusinessException("Aluno não possui número de registro acadêmico preenchido.");

            if (aluno.CodTipoCurso == null || aluno.CodTipoCurso == 0 )
                throw new BusinessException("Aluno não possui tipo de curso (contexto) preenchido.");

            if (String.IsNullOrWhiteSpace(aluno.Naturalidade))
                throw new BusinessException("Aluno não possui naturalidade preenchida.");

            if (String.IsNullOrWhiteSpace(aluno.NaturalidadePai))
                throw new BusinessException("Aluno não possui naturalidade do pai preenchida.");

            if (String.IsNullOrWhiteSpace(aluno.EstadoNatalPai))
                throw new BusinessException("Aluno não possui estado natal do pai preenchida.");

            if (String.IsNullOrWhiteSpace(aluno.NaturalidadeMae))
                throw new BusinessException("Aluno não possui naturalidade da mãe  preenchida.");

            if (String.IsNullOrWhiteSpace(aluno.EstadoNatalMae))
                throw new BusinessException("Aluno não possui estado natal da mãe  preenchida.");

            if (String.IsNullOrWhiteSpace(aluno.NaturalidadeAcad))
                throw new BusinessException("Aluno não possui naturalidade do responsável acadêmico  preenchida.");

            if (String.IsNullOrWhiteSpace(aluno.EstadoNatalAcad))
                throw new BusinessException("Aluno não possui estado natal do responsável acadêmico  preenchida.");
        }

        private void buscarTipoAluno(Aluno aluno)
        {
            Database dbSica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            using (DbCommand cmdSica = dbSica.GetSqlStringCommand(_queryBuscarTipoAluno))
            {
                dbSica.AddInParameter(cmdSica, "@ID_ALUNO", DbType.String, aluno.IdAluno);

                object tipoAluno = dbSica.ExecuteScalar(cmdSica);

                if (tipoAluno != null)
                    aluno.TipoAluno = tipoAluno.ToString().RemoveSpecialChars();
            }
        }

        /// <summary>
        /// Busca as informaçoes relacionadas ao contexto onde o aluno se encontra.
        /// </summary>
        /// <param name="aluno"></param>
        private void buscarTipoCurso(Aluno aluno)
        {
            Database dbSica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            using (DbCommand cmdSica = dbSica.GetSqlStringCommand(_queryBuscarCurso))
            {
                dbSica.AddInParameter(cmdSica, "@ID_ALUNO", DbType.String, aluno.IdAluno);

                using (IDataReader drTipoCurso = dbSica.ExecuteReader(cmdSica))
                {
                    while (drTipoCurso.Read())
                    {
                        aluno.CodTipoCurso = (new CursoDAO()).BuscaCursoPorID(Convert.ToInt32(drTipoCurso["ID_CURSO"])).CodTipoCurso;
                    }
                }
            }
        }


        private string buscarSobrenome(string nomeCompleto)
        {
            int primeiroEspaco = nomeCompleto.IndexOf(' ');

            return nomeCompleto.Substring(primeiroEspaco + 1, nomeCompleto.Length - primeiroEspaco - 1);
        }

        private string buscarEstadoNatal(string estadoNatal,string paisNatal)
        {
            if(!String.IsNullOrEmpty(estadoNatal))
                estadoNatal = estadoNatal.RemoveSpecialChars().ToUpper();

            if (!String.IsNullOrEmpty(paisNatal))
                paisNatal = paisNatal.RemoveSpecialChars().ToUpper();

            var estado = estados.FirstOrDefault(x => x.Codigo.RemoveSpecialChars().ToUpper().Equals(estadoNatal));

            if (estado == null) 
            {
                if(_debugMode)
                    _bgWorker.ReportProgress((int)_progress, String.Format("AVISO: Estado Natal não encontrado. Estado:{0}, País:{1}", estadoNatal, paisNatal));
                //throw new BusinessException(String.Format("Estado Natal não cadastrado. Estado:{0}, País:{1}", estadoNatal, paisNatal));

                estado = estados.FirstOrDefault(x => x.Codigo.RemoveSpecialChars().ToUpper().Equals("--"));
            }

            return estado.Codigo;
        }
    }
}
