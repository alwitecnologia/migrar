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

namespace Exportador.Academico.Turno
{

    public class ExportadorTurno : IExportador
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
        public ExportadorTurno()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorTurno(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorTurno(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTurnos = @"SELECT DISTINCT
                                            HORINI
                                            ,HORFIM
                                            ,TIPO
                                        FROM (

                                            SELECT
                                            H.ID_TURMA
                                            ,MIN(ENTRADA) HORINI
                                            ,MAX(SAIDA) HORFIM
                                            ,CASE WHEN COUNT(DISTINCT H.TURNO)>1 THEN 'I' ELSE
                                                (CASE MAX(H.TURNO)
                                                    WHEN 1 THEN 'M'
                                                    WHEN 2 THEN 'V'
                                                    WHEN 3 THEN 'N'
                                                 END
                                                )
                                            END AS TIPO
                                            FROM HORARIO H
                                            INNER JOIN AULA_HORARIO AH ON AH.TURNO=H.TURNO
                                                AND AH.AULA=H.AULA
                                            GROUP BY H.ID_TURMA

                                        )
                                        AS T";

        private string _queryTipoCursos = @"select codtipocurso from stipocurso";

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

            List<Turno> turnos = new List<Turno>();

            error = buscarTurnos(turnos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Turno), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, turnos);
        }

        private bool buscarTurnos(List<Turno> turnos)
        {
            bool error = false;

            Database rm = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            DbCommand rmCmd = rm.GetSqlStringCommand(_queryTipoCursos);

            IDataReader drTipoCursos = rm.ExecuteReader(rmCmd);

            List<int> tipoCursos = new List<int>();

            while (drTipoCursos.Read())
            {
                int tipoCurso = Convert.ToInt32(drTipoCursos["codtipocurso"]);

                tipoCursos.Add(tipoCurso);
            }


            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTurnos);

            double totalRecords = sica.ExecuteReader(sicaCmd).RowCount();

            IDataReader drTurnos = sica.ExecuteReader(sicaCmd);

            double processedRecords = 0;

            while (drTurnos.Read())
            {

                foreach (var tipoCurso in tipoCursos)
                {
                    Turno t = new Turno();

                    try
                    {
                        t = mapearTurno(drTurnos);

                        t.CodTipoCurso = tipoCurso;

                        turnos.Add(t);

                        processedRecords++;

                    }
                    catch (Exception ex)
                    {
                        error = true;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o turno: Código {0},Motivo:{1}", t.Nome, ex.Message));
                    }

                }

            }

            return error;

        }

        private Turno mapearTurno(IDataRecord drTurno)
        {
            Turno turno = new Turno();

            turno.Nome = buscarNome(drTurno);

            TimeSpan? entrada;
            if (drTurno["HORINI"] != DBNull.Value)
            {
                entrada = (TimeSpan)drTurno["HORINI"];
                turno.HoraInicio = String.Format("{0}:{1}", (entrada.Value.Hours.ToString()).PadLeft(2, '0'), (entrada.Value.Minutes.ToString()).PadLeft(2, '0'));
            }

            TimeSpan? saida;
            if (drTurno["HORFIM"] != DBNull.Value) 
            {
                saida = (TimeSpan)drTurno["HORFIM"];
                turno.HoraFim = String.Format("{0}:{1}", (saida.Value.Hours.ToString()).PadLeft(2, '0'), (saida.Value.Minutes.ToString()).PadLeft(2, '0'));
            }
            
            turno.Tipo = DBHelper.GetString(drTurno, "TIPO");

            turno.CodColigada = 1;
            turno.CodFilial = 1;

            return turno;
        }

        private string buscarNome(IDataRecord drTurno)
        {
            string tipo = (drTurno["TIPO"] == DBNull.Value) ? String.Empty : drTurno["TIPO"].ToString();

            TimeSpan tsHorIni = (TimeSpan)((drTurno["HORINI"] == DBNull.Value) ? String.Empty : drTurno["HORINI"]);
            string strHorIni = String.Format("{0}{1}", tsHorIni.Hours.ToString().PadLeft(2, '0'), tsHorIni.Minutes.ToString().PadLeft(2, '0'));

            TimeSpan tsHorFim = (TimeSpan)((drTurno["HORFIM"] == DBNull.Value) ? String.Empty : drTurno["HORFIM"]);
            string strHorFim = String.Format("{0}{1}", tsHorFim.Hours.ToString().PadLeft(2, '0'), tsHorFim.Minutes.ToString().PadLeft(2, '0'));

            return String.Format("{0}-{1}-{2}", tipo,strHorIni,strHorFim);
        }
    }
}
