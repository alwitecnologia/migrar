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
                                                ,USUARIO.CODIGO_FUNCIONARIO AS COD_PROF
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

            professores = DBHelper.GetAll<Professor>("SICA", _queryProfessores,String.Empty, Converter);

            FileHelperEngine engine = new FileHelperEngine(typeof(Professor), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, professores);
        }

        private Professor Converter(IDataReader drProfessor)
        {
            Professor p = new Professor();

            p.Nome = (drProfessor["NOME"] == DBNull.Value) ? String.Empty : drProfessor["NOME"].ToString();
            p.CodProf = (drProfessor["COD_PROF"] == DBNull.Value) ? String.Empty : drProfessor["COD_PROF"].ToString();
            p.DtNascimento = (DateTime)drProfessor.GetNullableDateTime("DTNASCIMENTO");
            p.CPF = (drProfessor["CPF"] == DBNull.Value) ? String.Empty : drProfessor["CPF"].ToString().Replace(".", String.Empty).Replace("/", String.Empty);
            p.CartIdentidade = (drProfessor["RG"] == DBNull.Value) ? String.Empty : drProfessor["RG"].ToString();
            p.Naturalidade = (drProfessor["NATURALIDADE"] == DBNull.Value) ? String.Empty : drProfessor["NATURALIDADE"].ToString();
            p.EstadoNatal = buscarEstadoNatal(drProfessor);

            p.CodColigada = 1;

            return p;
        }

        private bool buscarProfessores(List<Professor> professores)
        {
            bool error = false;

            try
            {
                professores = DBHelper.GetAll<Professor>("SICA", _queryProfessores,String.Empty, Converter);
            }
            catch (Exception ex)
            {
                error = true;

                throw;
            }

            return error;

            /*

            bool error = false;

            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryProfessores);

            double totalRecords = getCount();

            IDataReader drProfessores = sica.ExecuteReader(sicaCmd);

            double processedRecords = 0;

            while (drProfessores.Read())
            {

                Professor p = new Professor();

                try
                {
                    p = mapearProfessor(drProfessores);

                    professores.Add(p);

                    processedRecords++;

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o professor: Código {0},Motivo:{1}", p.NomeTurno, ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));


            }

            return error;

             */
        }

        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryProfessores);

            IDataReader drHorarios = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
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
