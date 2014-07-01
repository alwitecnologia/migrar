using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Exportador.Helpers;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.RH.Horario
{
    public class ExportadorHorarios : IExportador
    {
        #region Private Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool _debugMode;

        #endregion

        #region Public Properties

        /// <summary>
        /// Número de registros totais a ser retornados. 
        /// 0 para todos.
        /// </summary>
        public int RecordsCount
        {
            get { return _recordsToReturn; }
            set { _recordsToReturn = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
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
        public ExportadorHorarios()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHorarios(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHorarios(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryIdHorarios = @"
                                            select distinct
                                            codesc as CodHorario
                                            ,nomesc  as DescricaoHorario
                                            ,DATEADD(WEEK, DATEDIFF(WEEK, 0,DATEADD(DAY, 6 - DATEPART(DAY, GETDATE()), GETDATE())), 0) as DataBaseHorario
                                            from {schemaName}.r006esc escala
                                            order by codesc desc";

        private string _queryBatidasNormais = @"
                                            select distinct
	                                            horarioEscala.codesc as CodHorario
	                                            ,datepart(weekday,dateadd(day,horarioEscala.seqreg-2,turmas.datbas)) as IndiceDia
	                                            ,batidas.HorarioBatida1
	                                            ,batidas.HorarioBatida2
	                                            ,batidas.HorarioBatida3
	                                            ,batidas.HorarioBatida4
	                                            ,batidas.HorarioBatida5
	                                            ,batidas.HorarioBatida6
                                            from {schemaName}.r006hor horarioEscala
                                            inner join {schemaName}.r006tma turmas on horarioEscala.codesc=turmas.codesc
                                            inner join (
	                                            select codesc,min(codtma) codMin
	                                            from {schemaName}.r006tma
	                                            group by codesc
                                            ) as minTurma on minTurma.codesc=turmas.codesc
	                                            and minTurma.codMin=turmas.codtma
                                            inner join (
	                                            select 
	                                            codhor
	                                            ,max(marcacao.HorarioBatida1) HorarioBatida1
	                                            ,max(marcacao.HorarioBatida2) HorarioBatida2
	                                            ,max(marcacao.HorarioBatida3) HorarioBatida3
	                                            ,max(marcacao.HorarioBatida4) HorarioBatida4
	                                            ,max(marcacao.HorarioBatida5) HorarioBatida5
	                                            ,max(marcacao.HorarioBatida6) HorarioBatida6
	                                            from
	                                            (
		                                            select 
			                                            codhor
			                                            ,[1] as HorarioBatida1
			                                            ,[2] as HorarioBatida2
			                                            ,[3] as HorarioBatida3
			                                            ,[4] as HorarioBatida4
			                                            ,[5] as HorarioBatida5
			                                            ,[6] as HorarioBatida6
		                                            from {schemaName}.r004mhr pivot (max(horbat) for seqmar  in ([1],[2],[3],[4],[5],[6])) teste
	                                            ) as marcacao
	                                            group by codhor
                                            ) as batidas on batidas.codhor=horarioEscala.codhor
                                            order by horarioEscala.codesc
                                            ,datepart(weekday,dateadd(day,horarioEscala.seqreg-2,turmas.datbas))";

        private string _queryDiasSemBatidas = @"select distinct
	                                            horarioEscala.codesc as CodHorario
	                                            ,datepart(weekday,dateadd(day,horarioEscala.seqreg-1,turmas.datbas)) as IndiceDia
                                            from {schemaName}.r006hor horarioEscala
                                            inner join {schemaName}.r006tma turmas on horarioEscala.codesc=turmas.codesc
                                            inner join (
	                                            select codesc,min(codtma) codMin
	                                            from {schemaName}.r006tma
	                                            group by codesc
                                            ) as minTurma on minTurma.codesc=turmas.codesc
	                                            and minTurma.codMin=turmas.codtma
                                            where horarioEscala.codhor in (9996,9999)
                                            order by horarioEscala.codesc
	                                            ,datepart(weekday,dateadd(day,horarioEscala.seqreg-1,turmas.datbas))";

        #endregion

        public void Exportar()
        {

            List<object> registros = new List<object>();

            List<IdentificacaoHorario> horarios = new List<IdentificacaoHorario>();
            horarios.AddRange(buscarIdHorarios());

            registros.AddRange(buscarIdHorarios());

            List<BatidasNormais> batidas = new List<BatidasNormais>();
            batidas.AddRange(buscarBatidas());

            registros.AddRange(buscarBatidas());

            List<DiaSemBatida> diasSemBatida = new List<DiaSemBatida>();
            diasSemBatida.AddRange(buscarDiasSemBatida());

            registros.AddRange(buscarDiasSemBatida());

            _bgWorker.ReportProgress(0, "Iniciando geração arquivo...");

            MultiRecordEngine engine = new MultiRecordEngine(
                    new Type[]{typeof(IdentificacaoHorario)
                      , typeof(BatidasNormais)
                      , typeof(DiaSemBatida)});
            //engine.WriteFile(_filename, registros);

            if (System.IO.File.Exists(_filename))
                System.IO.File.Delete(_filename);

            salvarHorarios(horarios, batidas);
        }

        private void salvarHorarios(List<IdentificacaoHorario> horarios, List<BatidasNormais> batidas)
        {
            double processedRecords = 0;

            int progress = Convert.ToInt32(processedRecords / horarios.Count * 100);

            try
            {
                foreach (IdentificacaoHorario horario in horarios)
                {                    
                    //_bgWorker.ReportProgress(progress, String.Format("Cadastrando horário código {0}", horario.CodHorario));

                    HorarioDAO.CadastrarHorario(horario);
                                        
                    processedRecords++;
                    
                    progress = Convert.ToInt32(processedRecords / horarios.Count * 100);

                    String msg = String.Format("{0}% horários gerados.", (processedRecords / horarios.Count * 100).ToString());

                    _bgWorker.ReportProgress(progress, msg);                    

                    foreach (BatidasNormais batida in batidas.Where(batida => batida.CodHorario == horario.CodHorario))
                    {
                        HorarioDAO.CadastrarBatida(batida);
                    }  
                }
            }
            catch (Exception ex)
            {
                _bgWorker.ReportProgress(progress, String.Format("Não foi possível cadastrar a batida. {0}", ex.Message));
            }
        }

        private List<DiaSemBatida> buscarDiasSemBatida()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryDiasSemBatidas.Replace("{schemaName}", dbName));

            IDataReader drDiasSemBatida = database.ExecuteReader(command);

            List<DiaSemBatida> diasSemBatida = new List<DiaSemBatida>();

            while (drDiasSemBatida.Read())
            {
                DiaSemBatida diaSemBatida = new DiaSemBatida();

                diaSemBatida.CodHorario = drDiasSemBatida["CodHorario"].ToString();
                diaSemBatida.IndiceDia = Convert.ToInt32(drDiasSemBatida["IndiceDia"]);

                diasSemBatida.Add(diaSemBatida);
            }

            return diasSemBatida;
        }

        private List<BatidasNormais> buscarBatidas()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryBatidasNormais.Replace("{schemaName}", dbName));

            IDataReader drBatidas = database.ExecuteReader(command);

            List<BatidasNormais> lBatidas = new List<BatidasNormais>();

            while (drBatidas.Read())
            {
                BatidasNormais batida = new BatidasNormais();

                batida.CodHorario = drBatidas["CodHorario"].ToString();
                batida.IndiceDia = Convert.ToInt32(drBatidas["IndiceDia"]);
                batida.HorarioBatida1 = drBatidas["HorarioBatida1"] != DBNull.Value ? Convert.ToInt32(drBatidas["HorarioBatida1"]) : 0;
                batida.NaturezaBatida1 = 0;

                batida.HorarioBatida2 = drBatidas["HorarioBatida2"] != DBNull.Value ? Convert.ToInt32(drBatidas["HorarioBatida2"]) : 0;
                batida.NaturezaBatida2 = 1;

                batida.HorarioBatida3 = drBatidas["HorarioBatida3"] != DBNull.Value ? Convert.ToInt32(drBatidas["HorarioBatida3"]) : 0;
                batida.NaturezaBatida3 = 0;

                batida.HorarioBatida4 = drBatidas["HorarioBatida4"] != DBNull.Value ? Convert.ToInt32(drBatidas["HorarioBatida4"]) : 0;
                batida.NaturezaBatida4 = 1;

                batida.HorarioBatida5 = drBatidas["HorarioBatida5"] != DBNull.Value ? Convert.ToInt32(drBatidas["HorarioBatida5"]) : 0;
                batida.NaturezaBatida5 = 0;

                batida.HorarioBatida6 = drBatidas["HorarioBatida6"] != DBNull.Value ? Convert.ToInt32(drBatidas["HorarioBatida6"]) : 0;
                batida.NaturezaBatida6 = 1;

                batida.TipoBatida = 0;
                batida.Inicio = 0;
                batida.CodColigada = 1;
                batida.LetraDia = " ";
                batida.RecCreatedBy = "mestre";
                batida.RecModifiedBy = "mestre";

                lBatidas.Add(batida);
            }

            return lBatidas;
        }

        private List<IdentificacaoHorario> buscarIdHorarios()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryIdHorarios.Replace("{schemaName}", dbName));

            IDataReader drHorarios = database.ExecuteReader(command);

            List<IdentificacaoHorario> lIdHorarios = new List<IdentificacaoHorario>();

            while (drHorarios.Read())
            {
                IdentificacaoHorario horario = new IdentificacaoHorario();

                horario.CodHorario = drHorarios["CodHorario"].ToString();
                horario.DescricaoHorario = drHorarios["DescricaoHorario"].ToString();
                horario.DataBaseHorario = Convert.ToDateTime(drHorarios["DataBaseHorario"]).ToString("dd/MM/yyyy");

                horario.CodColigada = "1";
                horario.TipoHorario = "0";
                horario.Inativo = 0;
                horario.HorNoturno = 0;
                horario.ConsFeriado = 0;
                horario.HorarioJor = 0;
                horario.ConsFerDiaAnt = 0;
                horario.RecCreatedBy = "mestre";
                horario.RecModifiedBy = "mestre";

                lIdHorarios.Add(horario);
            }

            _bgWorker.ReportProgress(0, String.Format("{0} horário(s) encontrado(s).", lIdHorarios.Count.ToString()));

            return lIdHorarios;

        }

    }
}
