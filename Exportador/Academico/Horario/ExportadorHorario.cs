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

namespace Exportador.Academico.Horario
{
    public class ExportadorHorario : IExportador
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
        public ExportadorHorario()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHorario(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHorario(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHorariosSica = @"SELECT DISTINCT
                                            H.DIA_SEMANA
                                            ,AH.ENTRADA AS HORAINICIAL
                                            ,AH.SAIDA AS HORAFINAL
                                        FROM HORARIO H
                                        INNER JOIN AULA_HORARIO AH ON AH.TURNO=H.TURNO
                                            AND AH.AULA=H.AULA
                                        ORDER BY H.DIA_SEMANA,AH.AULA";

        private string _queryTipoCursos = @"select codtipocurso from stipocurso";

        private string _queryTurnos = @"SELECT DISTINCT 
                                            CODCOLIGADA
                                            ,CODFILIAL
                                            ,NOME
                                            ,HORINI
                                            ,HORFIM
                                            ,CODTIPOCURSO 
                                        FROM STURNO
                                        ORDER BY NOME";

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

            List<Horario> horarios = new List<Horario>();

            error = buscarHorarios(horarios);

            FileHelperEngine engine = new FileHelperEngine(typeof(Horario), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, horarios);
        }

        private bool buscarHorarios(List<Horario> horarios)
        {
            bool error = false;

            List<Exportador.Academico.Turno.Turno> turnos = DBHelper.GetAll("RM", _queryTurnos, String.Empty, TurnoConverter);

            List<HorarioSimples> lHorSimples = DBHelper.GetAll("SICA", _queryHorariosSica, String.Empty, HorarioSimplesConverter);

            foreach (Turno.Turno t in turnos)
            {
                TimeSpan inicioTurno = TimeSpan.Parse(t.HoraInicio);
                TimeSpan fimTurno = TimeSpan.Parse(t.HoraFim);

                List<HorarioSimples> horariosNoTurno = lHorSimples
                                                            .Where(hs => (hs.HoraInicial >= inicioTurno && hs.HoraFinal <= fimTurno))
                                                            .OrderBy(hs => hs.DiaSemana)
                                                            .ThenBy(hs => hs.Aula)
                                                            .ToList();

                var diasSemanas = horariosNoTurno.Select(ht => ht.DiaSemana).Distinct();//.Count();

                foreach (var dia in diasSemanas)
                {
                    var horariosNoDia = horariosNoTurno.Where(ht => ht.DiaSemana == dia).OrderBy(ht => ht.HoraInicial).ToList();

                    int aula = 1;

                    foreach (var hd in horariosNoDia)
                    {
                        Horario h = new Horario();

                        h.CodColigada = t.CodColigada;
                        h.CodFilial = t.CodFilial;
                        h.CodTipoCurso = t.CodTipoCurso;
                        h.NomeTurno = t.Nome;

                        h.Aula = aula.ToString();

                        h.DiaSemana = hd.DiaSemana;
                        h.HoraInicial = String.Format("{0}:{1}", (hd.HoraInicial.Hours.ToString()).PadLeft(2, '0'), (hd.HoraInicial.Minutes.ToString()).PadLeft(2, '0'));
                        h.HoraFinal = String.Format("{0}:{1}", (hd.HoraFinal.Hours.ToString()).PadLeft(2, '0'), (hd.HoraFinal.Minutes.ToString()).PadLeft(2, '0'));

                        horarios.Add(h);

                        aula++;
                    }
                }            
            }

            return error;

        }

        private Horario mapearHorario(IDataRecord drTipoCurso, IDataRecord drTurno)
        {
            Horario horario = new Horario();

            horario.CodTipoCurso = Convert.ToInt32(drTipoCurso["codtipocurso"]);
            horario.NomeTurno = buscarNome(drTurno);
            horario.DiaSemana = Convert.ToInt32(drTurno["dia_semana"]);
            horario.HoraInicial = (drTurno["ENTRADA"] == DBNull.Value) ? String.Empty : drTurno["ENTRADA"].ToString();
            horario.HoraFinal = (drTurno["SAIDA"] == DBNull.Value) ? String.Empty : drTurno["SAIDA"].ToString();
            horario.Aula = (drTurno["aula"] == DBNull.Value) ? String.Empty : drTurno["aula"].ToString();

            horario.CodColigada = 1;
            horario.CodFilial = 1;

            return horario;
        }

        private string buscarNome(IDataRecord drTurno)
        {
            string turno = (drTurno["TURNO"] == DBNull.Value) ? String.Empty : drTurno["TURNO"].ToString();

            return String.Format("T{0}", turno);
        }

        private Turno.Turno TurnoConverter(IDataReader drTurno)
        {
            Turno.Turno t = new Turno.Turno();

            t.CodTipoCurso = (int)drTurno.GetNullableInt32("CODTIPOCURSO");
            t.CodColigada = (int)drTurno.GetNullableInt32("CODCOLIGADA");
            t.CodFilial = (int)drTurno.GetNullableInt32("CODFILIAL");
            t.Nome = drTurno.GetString("NOME");
            t.HoraInicio = drTurno.GetString("HORINI");
            t.HoraFim = drTurno.GetString("HORFIM");

            return t;
        }

        private HorarioSimples HorarioSimplesConverter(IDataReader drHorSimples)
        {
            HorarioSimples hs = new HorarioSimples();

            hs.DiaSemana = (int)drHorSimples.GetNullableInt32("DIA_SEMANA");
            hs.HoraInicial = (TimeSpan)drHorSimples["HORAINICIAL"];
            hs.HoraFinal = (TimeSpan)drHorSimples["HORAFINAL"];

            return hs;
        }
    }

    public struct HorarioSimples
    {
        public int DiaSemana;
        public string Aula;
        public TimeSpan HoraInicial;
        public TimeSpan HoraFinal;
    }

}
