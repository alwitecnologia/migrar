using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.DAO;
using Exportador.Helpers;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.RH.Historicos
{
    public class ExportadorHistEnderecos: IExportador
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
        public ExportadorHistEnderecos()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHistEnderecos(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHistEnderecos(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHistContribuicoes = @"select * from (select
                                                       funcionario.numcad as Id
                                                       --campos obrigatórios
                                                          ,case when funcionario.tipcol=2 
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
                                                        ,funcionario.datadm as DataAdmissao
                                                        ,ficha.endrua as Rua
                                                        ,ficha.endnum as Numero
                                                        ,ficha.endcpl as Complemento
                                                        ,bairroEndereco.nombai as Bairro
                                                        ,ficha.codest as Estado
                                                        ,cidadeEndereco.nomcid as Cidade
                                                        ,ficha.endcep as CEP
                                                        ,paisEndereco.nompai as Pais
                                                    from {schemaName}.r034fun as funcionario
                                                    left join {schemaName}.r034cpl as ficha on ficha.numemp=funcionario.numemp
                                                        and ficha.tipcol=funcionario.tipcol
                                                        and ficha.numcad=funcionario.numcad
                                                    left join {schemaName}.r074bai bairroEndereco on bairroEndereco.codcid=ficha.codcid
                                                        and bairroEndereco.codbai=ficha.codbai
                                                    left join {schemaName}.r074cid cidadeEndereco on cidadeEndereco.codcid=ficha.codcid
                                                    left join {schemaName}.r074pai paisEndereco on paisEndereco.codpai=ficha.codpai
                                                    inner join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                                    where secao.taborg=5
                                                ) as histFunc
                                                order by chapa";

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

            List<Endereco> histEnderecos = new List<Endereco>();

            error = buscarHistoricoEnderecos(histEnderecos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Endereco), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, histEnderecos);
        }

        private bool buscarHistoricoEnderecos(List<Endereco> histEnderecos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryHistContribuicoes.Replace("{schemaName}", dbName));

            IDataReader drContribuicao = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drContribuicao.Read())
            {
                Endereco histEnd = new Endereco();

                try
                {
                    processedRecords++;

                    histEnd.CodPessoa = FuncionarioDAO.BuscarCodPessoa(drContribuicao["Chapa"].ToString());
                    histEnd.DtMudanca = Convert.ToDateTime(drContribuicao["DataAdmissao"]);
                    histEnd.Rua = drContribuicao["Rua"].ToString();
                    histEnd.Numero = drContribuicao["Numero"].ToString();
                    histEnd.Complemento = drContribuicao["Complemento"].ToString();
                    histEnd.Bairro = drContribuicao["Bairro"].ToString();
                    histEnd.Estado = drContribuicao["Estado"].ToString();
                    histEnd.Cidade = drContribuicao["Cidade"].ToString(); 


                    histEnderecos.Add(histEnd);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração: Chapa {0}, DtMudança {1}. Motivo:{2}", histEnd.CodPessoa, Convert.ToDateTime(histEnd.DtMudanca).ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    }
}
