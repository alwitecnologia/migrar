using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.Interface;
using Exportador.Helpers;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data;
using Exportador.Academico.Curso;
using Exportador.DAO;

namespace Exportador.Academico.PeriodoLetivo
{
    public class ExportadorPeriodoLetivo: IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private bool _debugMode;
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
        public ExportadorPeriodoLetivo()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPeriodoLetivo(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPeriodoLetivo(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryPeriodos = @"SELECT DISTINCT
                                                MC.ANO
                                                ,MC.SEMESTRE
                                                ,NIVEL_CURSO.NOME AS NIVELCURSO
                                                ,CURSO.NOME AS NOMECURSO
                                            FROM MATRICULA_CURSO MC
                                            INNER JOIN CURSO ON CURSO.ID=MC.ID_CURSO
                                            INNER JOIN NIVEL_CURSO ON NIVEL_CURSO.ID=CURSO.ID_NIVEL_CURSO
                                        ORDER BY ANO,SEMESTRE";

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

            List<PeriodoLetivo> pLetivos = new List<PeriodoLetivo>();

            error = buscarPeriodos(pLetivos);

            FileHelperEngine engine = new FileHelperEngine(typeof(PeriodoLetivo), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, pLetivos.Distinct(new PeriodoLetivoCompare()).ToList());
        }

        private bool buscarPeriodos(List<PeriodoLetivo> pLetivos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand command = database.GetSqlStringCommand(_queryPeriodos);

            double totalRecords = database.ExecuteReader(command).RowCount();

            IDataReader drPeriodos = database.ExecuteReader(command);

            double processedRecords = 0;

            while (drPeriodos.Read())
            {
                PeriodoLetivo pLetivo = new PeriodoLetivo();

                try
                {
                    pLetivo = mapearPeriodoLetivo(drPeriodos);

                    pLetivos.Add(pLetivo);

                    processedRecords++;

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o período letivo: Código {0},Motivo:{1}", pLetivo.CodPeriodoLetivo, ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }

        private PeriodoLetivo mapearPeriodoLetivo(IDataRecord drPeriodos)
        {
            PeriodoLetivo pLetivo = new PeriodoLetivo();

            int anoAtual = DateTime.Now.Year;
            int semestreAtual = (DateTime.Now.Month / 6) + 1;

            int anoPeriodo = (int)drPeriodos.GetNullableInt32("ANO");
            int semestrePeriodo = (int)drPeriodos.GetNullableInt32("SEMESTRE");

            int codTipoCurso = _cursoDAO.buscarTipoCurso((string)drPeriodos["NIVELCURSO"], (string)drPeriodos["NOMECURSO"]);


            pLetivo.Encerrado = ((anoPeriodo == anoAtual) || (semestrePeriodo == semestreAtual));
            pLetivo.CodTipoCurso = codTipoCurso;

            pLetivo.CodPeriodoLetivo = String.Format("{0}-{1}/{2}", (codTipoCurso == 1 ? "SUP" :
                                                                       codTipoCurso == 2 ? "PGM" :
                                                                       codTipoCurso == 3 ? "EXT" :
                                                                       codTipoCurso == 7 ? "EXT" : String.Empty), anoPeriodo.ToString(), semestrePeriodo.ToString());


            pLetivo.Descricao = String.Format("Período {0}/{1}", anoPeriodo.ToString(), semestrePeriodo.ToString());


            pLetivo.CodColigada = 1;
            pLetivo.CodFilial = 1;

            return pLetivo;
        }
    }

    public class PeriodoLetivoCompare : IEqualityComparer<PeriodoLetivo>
    {
        public bool Equals(PeriodoLetivo x, PeriodoLetivo y)
        {
            if ((x.CodColigada == y.CodColigada) && (x.CodPeriodoLetivo == y.CodPeriodoLetivo) && (x.CodTipoCurso == y.CodTipoCurso))
            {
                return true;
            }
            else { return false; }
        }

        public int GetHashCode(PeriodoLetivo codeh)
        {
            return 0;
        }

    }
}
