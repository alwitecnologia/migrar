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
using Exportador.Academico.Pessoa;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using System.Data.SqlClient;

namespace Exportador.Academico.Professor
{
    public class ExportadorProfessor : IExportador
    {

        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private bool _debugMode;
        private List<Estado> estados;
        private string _sicaConnStr = ConfigurationManager.ConnectionStrings["SICA"].ConnectionString;
        private string _rubiConnStr = ConfigurationManager.ConnectionStrings["VetoRH"].ConnectionString;

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
        public ExportadorProfessor()
        {
            EstadoDAO estDAO = new EstadoDAO();
            estados = estDAO.buscarTodas();
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorProfessor(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorProfessor(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;

            EstadoDAO estDAO = new EstadoDAO();
            estados = estDAO.buscarTodas();
        }

        #endregion

        #region Queries

        private string _queryProfessores = @"SELECT
                                                USUARIO.NOME
                                                ,USUARIO.DATA_NASCIMENTO AS DTNASCIMENTO
                                                ,USUARIO.ID AS COD_PROF
                                                ,USUARIO.CPF
                                                ,USUARIO.RG
                                                ,COALESCE(CL.NOME,'Indefinido') as NATURALIDADE
                                                ,COALESCE(CU.SIGLA,'--') as ESTADONATAL
                                            FROM PROFESSOR
                                                INNER JOIN USUARIO ON USUARIO.ID=PROFESSOR.ID_USUARIO
                                                LEFT JOIN CEP_LOCALIDADES CL ON (USUARIO.ID_NATURALIDADE = CL.ID)
                                                LEFT JOIN CEP_UFS CU ON (CL.ID_CEP_UF = CU.ID)
                                            WHERE
                                                USUARIO.ID NOT IN (0,27144)
                                                AND USUARIO.NOME IS NOT NULL";
        private string _queryCount = @"SELECT
                                                COUNT(*)
                                            FROM PROFESSOR
                                                INNER JOIN USUARIO ON USUARIO.ID=PROFESSOR.ID_USUARIO
                                                LEFT JOIN CEP_LOCALIDADES CL ON (USUARIO.ID_NATURALIDADE = CL.ID)
                                                LEFT JOIN CEP_UFS CU ON (CL.ID_CEP_UF = CU.ID)
                                            WHERE
                                                USUARIO.ID NOT IN (0,27144)
                                                AND USUARIO.NOME IS NOT NULL";

        private string _queryPessoasRM = @"select 
	                                CPF, 
	                                NOME,
	                                DTNASCIMENTO,
	                                CARTIDENTIDADE,
	                                UFCARTIDENT,
	                                NATURALIDADE, 
	                                ESTADONATAL,
	                                CARTEIRATRAB,
	                                SERIECARTTRAB,
	                                UFCARTTRAB
                                from PPESSOA";

        private string _queryTitulacao = @"select case usu_tiptit
	                                            when 0 then null
	                                            when 5 then 4
	                                            else usu_tiptit
	                                            end
                                            from vetorh.r034fun
                                            where REPLICATE('0',(11-LEN(numcpf)))+cast(numcpf as varchar(11))=@numcpf";

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

            List<Professor> professores = new List<Professor>();

            List<Professor> pessoaRM = new List<Professor>();

            buscarPessoasRM(pessoaRM);
            
            error = buscarProfessores(professores, pessoaRM);

            FileHelperEngine engine = new FileHelperEngine(typeof(Professor), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, professores);
        }

        private void buscarPessoasRM(List<Professor> pessoaRM)
        {

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            DbCommand command = database.GetSqlStringCommand(_queryPessoasRM);

            _bgWorker.ReportProgress(0, "Executando consulta de pessoas...");

            IDataReader drPessoas = database.ExecuteReader(command);

            _bgWorker.ReportProgress(0, "Montando objetos de PESSOAS DO RM...");

            while (drPessoas.Read())
            {
                Professor prof = new Professor();

                try
                {

                    prof.CPF = drPessoas["CPF"].ToString();
                    prof.Nome = drPessoas["NOME"].ToString();
                    prof.DtNascimento = Convert.ToDateTime(drPessoas["DTNASCIMENTO"].ToString());
                    prof.CartIdentidade = drPessoas["CARTIDENTIDADE"].ToString();
                    prof.UFCartIdentidade = drPessoas["UFCARTIDENT"].ToString();
                    prof.Naturalidade = drPessoas["NATURALIDADE"].ToString();
                    prof.EstadoNatal = drPessoas["ESTADONATAL"].ToString();
                    prof.CarteiraTrab = drPessoas["CARTEIRATRAB"].ToString();
                    prof.SerieCartTrab = drPessoas["SERIECARTTRAB"].ToString();
                    prof.UFCartTrab = drPessoas["UFCARTTRAB"].ToString();
                    
                    pessoaRM.Add(prof);
                }
                catch (Exception ex)
                {

                    _bgWorker.ReportProgress(0, String.Format("Não foi possível exportar o professor - Erro: {0}", ex.Message));
                }

            }
        }

        private Professor Converter(IDataReader drProfessor, List<Professor> pessoaRM, double records, double totalRecords)
        {
            Professor p = new Professor();

            var result = pessoaRM.Where(f => f.CPF == drProfessor["CPF"].ToString());

            if (result.Count() >= 1)
            {
                var value = result.First();

                p.Nome = value.Nome;
                p.Chapa = FuncionarioDAO.buscarChapa(drProfessor["CPF"].ToString(), p.Nome);
                p.CodProf = drProfessor["COD_PROF"].ToString();
                p.DtNascimento = value.DtNascimento;
                p.CPF = value.CPF;
                p.CartIdentidade = value.CartIdentidade;
                p.UFCartIdentidade = value.UFCartIdentidade;
                p.Naturalidade = value.Naturalidade;
                p.EstadoNatal = value.EstadoNatal;
                p.Nome = value.Nome;
                p.CarteiraTrab = value.CarteiraTrab;
                p.SerieCartTrab = value.SerieCartTrab;
                p.UFCartTrab = value.UFCartTrab;

                p.Titulacao = buscarTitulacao(p.CPF);                

                p.CodColigada = 1;                
            }
            else
            {
                _bgWorker.ReportProgress(Convert.ToInt32(records / totalRecords * 100), String.Format("Funcionário não possui cadastro no RM, cpf: {0}, nome: {1}", drProfessor["CPF"].ToString(), drProfessor["NOME"].ToString()));
            } 

            return p;                       
        }

        private string buscarTitulacao(string cpf)
        {
            string titulacao = string.Empty;

            using (DbConnection connection = new SqlConnection(_rubiConnStr))
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = _queryTitulacao;
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();

                    command.Parameters.Add(new SqlParameter("numcpf", cpf));

                    var vTitulacao = command.ExecuteScalar();

                    if (vTitulacao != null)
                    {
                        titulacao = vTitulacao.ToString();    
                    }                    

                }
                catch (Exception ex)
                {
                    error = true;
                    _bgWorker.ReportProgress(0, string.Format("Erro desconhecido: {0}", ex.Message));
                }
            }

            return titulacao;
        }

        private bool buscarProfessores(List<Professor> professores, List<Professor> pessoaRM)
        {
            bool error = false;

            double totalRecords = getCount();

            _bgWorker.ReportProgress(0, string.Format("Gerando Pessoas no SICA..."));

            using (DbConnection connection = new FbConnection(_sicaConnStr))
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = _queryProfessores;
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();

                    IDataReader drProfessores = command.ExecuteReader();

                    double processedRecords = 0;

                    while (drProfessores.Read())
                    {
                        processedRecords++;

                        Professor p = new Professor();

                        p = Converter(drProfessores, pessoaRM, processedRecords, totalRecords);

                        if (p.CPF != null)

                            professores.Add(p);
                        else
                        {
                            _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), "CPF em branco, nome: " + drProfessores["NOME"]);
                        }

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }

                    drProfessores.Close();
                }
                catch (Exception ex)
                {
                    error = true;
                    _bgWorker.ReportProgress(0, string.Format("Erro desconhecido: {0}", ex.Message));
                }
                return error;
            } 
        }

        private double getCount()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SICA"].ConnectionString;

            double count=0;

            using (DbConnection connection = new FbConnection(connStr))
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                command.CommandText = _queryCount;
                command.CommandType = CommandType.Text;

                return Convert.ToDouble(command.ExecuteScalar());
            }

            return count;
        }

        private Professor mapearProfessor(IDataRecord drProfessor)
        {
            Professor p = new Professor();

            p.Nome = (drProfessor["NOME"] == DBNull.Value) ? String.Empty : drProfessor["NOME"].ToString();
            p.DtNascimento = (DateTime)drProfessor.GetNullableDateTime("DTNASCIMENTO");
            p.CPF = (drProfessor["CPF"] == DBNull.Value) ? String.Empty : drProfessor["CPF"].ToString().Replace(".",String.Empty).Replace("/",String.Empty);
            p.CartIdentidade = (drProfessor["RG"] == DBNull.Value) ? String.Empty : drProfessor["RG"].ToString();
            p.Naturalidade = (drProfessor["NATURALIDADE"] == DBNull.Value) ? String.Empty : drProfessor["NATURALIDADE"].ToString();
            p.EstadoNatal = buscarEstadoNatal(drProfessor);

            p.CodColigada = 1;

            return p;
        }

        private string buscarEstadoNatal(IDataRecord drProfessor)
        {
            string estadoNatal = (DBNull.Value == drProfessor["ESTADONATAL"]) ? String.Empty : ((string)drProfessor["ESTADONATAL"]).ToUpper();
            string paisNatal = (DBNull.Value == drProfessor["NATURALIDADE"]) ? String.Empty : ((string)drProfessor["NATURALIDADE"]).ToUpper();

            var estado = estados.FirstOrDefault(x => x.Codigo.RemoveSpecialChars().ToUpper().Equals(estadoNatal));

            if (estado == null)
                throw new BusinessException(String.Format("Estado Natal não cadastrado. Estado:{0}, País:{1}", estadoNatal, paisNatal));

            return estado.Codigo;
        }
    }
}
