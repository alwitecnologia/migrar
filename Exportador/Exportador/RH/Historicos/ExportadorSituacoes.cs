using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.Helpers;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.RH.Historicos
{
    public class ExportadorSituacoes: IExportador
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
        public ExportadorSituacoes()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorSituacoes(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorSituacoes(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _querySituacoes1 = @"with afastamentos as (

	                                            select r038afa.numemp 
		                                            ,r038afa.tipcol 
		                                            ,r038afa.numcad
		                                            ,r038afa.datafa
		                                            ,r038afa.horafa
		                                            ,r038afa.sitafa 
	                                            from {schemaName}.r038afa
	
                                            )

                                            select
      case when funcionario.tipcol=2 
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
                                            ,DATEADD(minute,histSit.horafa,histSit.datafa) as DtMudanca
                                            ,'1' as CodMotivoMudanca
                                            ,situacoes.dessit as NovaSituacao
                                            from afastamentos histSit
                                            inner join {schemaName}.r034fun funcionario on funcionario.numemp=histSit.numemp
                                                and funcionario.tipcol=histSit.tipcol
                                                and funcionario.numcad=histSit.numcad
                                            inner join {schemaName}.r010sit situacoes on situacoes.codsit=histSit.sitafa
                                            left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                            where secao.taborg=5";

        private string _querySituacoes2 = @"with afastamentos as (
                
	                                            select fun.numemp
		                                            ,fun.tipcol
		                                            ,fun.numcad
		                                            ,fun.datadm as datafa
		                                            ,0 as horafa
		                                            ,1 as sitafa
	                                            from {schemaName}.r034fun fun
	                                            inner join {schemaName}.r038afa afa
		                                            on fun.numemp = afa.numemp 
			                                            and fun.tipcol = afa.tipcol 
			                                            and fun.numcad = afa.numcad  
	                                            inner join (
				                                            select min(datafa) minDatafa
					                                            ,numemp
					                                            ,tipcol
					                                            ,numcad
				                                            from {schemaName}.r038afa
				                                            group by numemp
					                                            ,tipcol
					                                            ,numcad
				                                            ) as b on b.numcad=fun.numcad
					                                            and b.numemp=fun.numemp
					                                            and b.tipcol=fun.tipcol
	                                            and b.minDatafa=afa.datafa

                                            )

                                            select
      case when funcionario.tipcol=2 
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
                                            ,DATEADD(minute,histSit.horafa,histSit.datafa) as DtMudanca
                                            ,'1' as CodMotivoMudanca
                                            ,situacoes.dessit as NovaSituacao
                                            from afastamentos histSit
                                            inner join {schemaName}.r034fun funcionario on funcionario.numemp=histSit.numemp
                                                and funcionario.tipcol=histSit.tipcol
                                                and funcionario.numcad=histSit.numcad
                                            inner join {schemaName}.r010sit situacoes on situacoes.codsit=histSit.sitafa
                                            left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                            where secao.taborg=5";

        private string _querySituacoes3 = @"with afastamentos as (
    
	                                            select a.numemp
		                                            ,a.tipcol
		                                            ,a.numcad
		                                            ,a.datadm as datafa
		                                            ,0 as horafa
		                                            ,1 as sitafa
	                                            from {schemaName}.r034fun a
	                                            left join (
				                                            select numemp
					                                            ,tipcol
					                                            ,numcad
				                                            from {schemaName}.r038afa
				                                            ) b on a.numemp = b.numemp 
						                                            and a.tipcol = b.tipcol 
						                                            and a.numcad = b.numcad
	                                            where b.numcad is null

                                            )

                                            select
      case when funcionario.tipcol=2 
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
                                            ,DATEADD(minute,histSit.horafa,histSit.datafa) as DtMudanca
                                            ,'1' as CodMotivoMudanca
                                            ,situacoes.dessit as NovaSituacao
                                            from afastamentos histSit
                                            inner join {schemaName}.r034fun funcionario on funcionario.numemp=histSit.numemp
                                                and funcionario.tipcol=histSit.tipcol
                                                and funcionario.numcad=histSit.numcad
                                            inner join {schemaName}.r010sit situacoes on situacoes.codsit=histSit.sitafa
                                            left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                            where secao.taborg=5";

        private string _querySituacoes4 = @"with afastamentos as (

                                                select 
	                                                a.numemp
	                                                ,a.tipcol
	                                                ,a.numcad
	                                                ,dateadd(day,1,a.datter) as datafa 
	                                                ,0 as horafa
	                                                ,1 as sitafa
                                                from {schemaName}.r038afa a
                                                inner join {schemaName}.r038afa b 
                                                    on a.numemp = b.numemp 
                                                    and a.tipcol = b.tipcol 
                                                    and a.numcad = b.numcad 
                                                    and b.datafa <> dateadd(day,1,a.datter) 
                                                    and b.datafa <> a.datter 
                                                left join {schemaName}.r038afa c
                                                    on a.numemp = c.numemp 
                                                    and a.numcad = c.numcad 
                                                    and a.tipcol = c.tipcol 
                                                    and c.datter = dateadd(day,1,a.datter)
                                                inner join (
                                                            select max(d.datafa) maxDatafa
                                                                ,numemp
                                                                ,tipcol
                                                                ,numcad
                                                            from {schemaName}.r038afa d 
                                                            group by numemp
                                                                ,tipcol
                                                                ,numcad
                                                            ) as d on d.numcad=a.numcad
                                                            and d.tipcol=a.tipcol
                                                            and d.numemp=a.numemp
                                                            and d.maxDatafa > a.datafa

                                                where c.numcad is null 
                                                and dateadd(day,1,a.datter) < convert(datetime,convert(int,vetorh.getdbdatetime())) 
                                                and b.datafa = (select min(d1.datafa) 
                                                                from {schemaName}.r038afa d1 
                                                                where d1.numemp = b.numemp 
                                                                    and d1.tipcol = b.tipcol 
                                                                    and d1.numcad = b.numcad 
                                                                    and d1.datafa > a.datter
                                                                ) 
                                                ) 

                                                select
      case when funcionario.tipcol=2 
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
                                                ,DATEADD(minute,histSit.horafa,histSit.datafa) as DtMudanca
                                                ,'1' as CodMotivoMudanca
                                                ,situacoes.dessit as NovaSituacao
                                                from afastamentos histSit
                                                inner join {schemaName}.r034fun funcionario on funcionario.numemp=histSit.numemp
                                                    and funcionario.tipcol=histSit.tipcol
                                                    and funcionario.numcad=histSit.numcad
                                                inner join {schemaName}.r010sit situacoes on situacoes.codsit=histSit.sitafa
                                                left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                                where secao.taborg=5";


        private string _querySituacoes5 = @"with afastamentos as (

                                                select a.numemp
                                                    ,a.tipcol
                                                    ,a.numcad
                                                    ,dateadd(day,1,a.datter) as datafa
                                                    ,0 as horafa
                                                    ,1 as sitafa
                                                from {schemaName}.r038afa a 
                                                inner join (
                                                            select MAX(datafa) as maxDatafa
                                                                ,numemp
                                                                ,tipcol
                                                                ,numcad
                                                            from {schemaName}.r038afa
                                                            group by numemp
                                                                ,tipcol
                                                                ,numcad
                                                            ) b
                                                            on a.numemp = b.numemp 
                                                            and a.tipcol = b.tipcol 
                                                            and a.numcad = b.numcad
                                                            and b.maxDatafa=a.datafa

                                                where a.datter < convert(datetime,convert(int,vetorh.getdbdatetime())) 
                                                and a.sitafa <> 7

                                                )

                                                select
      case when funcionario.tipcol=2 
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
                                                ,DATEADD(minute,histSit.horafa,histSit.datafa) as DtMudanca
                                                ,'1' as CodMotivoMudanca
                                                ,situacoes.dessit as NovaSituacao
                                                from afastamentos histSit
                                                inner join {schemaName}.r034fun funcionario on funcionario.numemp=histSit.numemp
                                                    and funcionario.tipcol=histSit.tipcol
                                                    and funcionario.numcad=histSit.numcad
                                                inner join {schemaName}.r010sit situacoes on situacoes.codsit=histSit.sitafa
                                                left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                                where secao.taborg=5";

        private string _querySituacoes = @"select
      case when funcionario.tipcol=2 
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
                                            ,DATEADD(minute,histSit.horafa,histSit.datafa) as DtMudanca
                                            ,'1' as CodMotivoMudanca
                                            ,situacoes.dessit as NovaSituacao
                                            from {schemaName}.vbi_hissit histSit
                                            inner join {schemaName}.r034fun funcionario on funcionario.numemp=histSit.numemp
	                                            and funcionario.tipcol=histSit.tipcol
	                                            and funcionario.numcad=histSit.numcad
                                            inner join {schemaName}.r010sit situacoes on situacoes.codsit=histSit.sitafa
                                            left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                            where secao.taborg=5";

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

            List<Situacoes> lSituacoes = new List<Situacoes>();

            error = buscarSituacoes(lSituacoes);

            FileHelperEngine engine = new FileHelperEngine(typeof(Situacoes), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, lSituacoes);
        }

        private bool buscarSituacoes(List<Situacoes> lSituacoes)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            error = buscarSituacoes(lSituacoes, database, _querySituacoes1.Replace("{schemaName}", dbName));

            if(!error)
                error = buscarSituacoes(lSituacoes, database, _querySituacoes2.Replace("{schemaName}", dbName));
            
            if (!error)
                error = buscarSituacoes(lSituacoes, database, _querySituacoes3.Replace("{schemaName}", dbName));

            if (!error)
                error = buscarSituacoes(lSituacoes, database, _querySituacoes4.Replace("{schemaName}", dbName));

            if (!error)
                error = buscarSituacoes(lSituacoes, database, _querySituacoes5.Replace("{schemaName}", dbName));

            return error;

        }

        private bool buscarSituacoes(List<Situacoes> lSituacoes, Database database, string _query)
        {
            DbCommand command = database.GetSqlStringCommand(_query);

            IDataReader drSituacoes = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drSituacoes.Read())
            {
                Situacoes altSituacao = new Situacoes();

                try
                {
                    processedRecords++;

                    altSituacao.Chapa = drSituacoes["Chapa"].ToString();
                    altSituacao.CodMotivoMudanca = drSituacoes["CodMotivoMudanca"].ToString();
                    altSituacao.DtMudanca = Convert.ToDateTime(drSituacoes["DtMudanca"]);
                    altSituacao.NovaSituacao = drSituacoes["NovaSituacao"].ToString();

                    lSituacoes.Add(altSituacao);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração de situação: Chapa {0}, DtMudanca {1}. Motivo:{2}", altSituacao.Chapa, altSituacao.DtMudanca.ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }
            return error;
        }
    }
}
