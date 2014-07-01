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
using Exportador.RH.Funcionario;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.RH.Dependente
{
    public class ExportadorDependente: IExportador
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
        public ExportadorDependente()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorDependente(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorDependente(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryDependentes = @"
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
	,dependente.coddep as NumDependente
	,dependente.nomdep as Nome
	,dependente.numcpf as CPF
	,dependente.datnas as DtNascimento
	,dependente.tipsex as Sexo
    ,case funcionario.estciv
        when 1 then 'Solteiro'
        when 2 then 'Casado'
        when 3 then 'Divorciado'
        when 4 then 'Viúvo'
        when 5 then 'Concubinato'
        when 6 then 'Separado'
        when 7 then 'União Estável'
        when 9 then 'Outros'
    end		 
    as EstadoCivil
	,dependente.locnas as LocalNascimento
	,dependente.nomcar as NomeCartorio
	,dependente.numreg as NumRegistro
	,dependente.numliv as NumLivroRegistro
	,dependente.numfol as NumFolhaRegistro
	,case when FLOOR((CAST (GetDate() AS INTEGER) - CAST(dependente.datnas AS INTEGER)) / 365.25) < 21 then 1 else 0 end AS IncideIRRF
    ,case dependente.grapar
	        when 1 then 'Filho(a)'
	        when 2 then 'Cônjuge'
	        when 3 then 
		        case when dependente.tipsex='M' then 'Pai' else 'Mãe' end
	        when 6 then 'Sobrinho(a)'
	        when 8 then 'Neto(a)'	
	        when 10 then 'Genro/Nora'
	        when 11 then 'Enteado(a)'
	        when 99 then 'Outros'
        end
        as GrauParentesco
	,dependente.entcer as DtEntregaCert
	,case when FLOOR((CAST (GetDate() AS INTEGER) - CAST(dependente.datnas AS INTEGER)) / 365.25) < 14 then '1' else '0' end AS IncideSalFamilia
from {schemaName}.r036dep as dependente
inner join {schemaName}.r034fun funcionario on funcionario.numemp=dependente.numemp
    and funcionario.tipcol=dependente.tipcol
    and funcionario.numcad=dependente.numcad
inner join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
where secao.taborg=5
";

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

            List<Dependente> dependentes = new List<Dependente>();

            error = buscarDependentes(dependentes);

            FileHelperEngine engine = new FileHelperEngine(typeof(Dependente), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, dependentes);
        }

        private bool buscarDependentes(List<Dependente> dependentes)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryDependentes.Replace("{schemaName}", dbName));

            IDataReader drDependentes = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drDependentes.Read())
            {
                Dependente dependente = new Dependente();

                try
                {
                    processedRecords++;

                    dependente.Chapa = drDependentes["Chapa"].ToString();
                    dependente.NumDependente = Convert.ToInt32(drDependentes["NumDependente"]);
                    dependente.Nome = drDependentes["Nome"].ToString();
                    dependente.CPF = drDependentes["CPF"].ToString();
                    dependente.DtNasc = Convert.ToDateTime(drDependentes["DtNascimento"]);
                    dependente.Sexo = drDependentes["Sexo"].ToString();
                    dependente.EstadoCivil = buscarEstadoCivil(drDependentes["EstadoCivil"].ToString());
                    dependente.LocalNascimento = drDependentes["LocalNascimento"].ToString();
                    dependente.NomeCartorio = drDependentes["NomeCartorio"].ToString();
                    dependente.NumRegistro = drDependentes["NumRegistro"].ToString();
                    dependente.NumLivroRegistro = drDependentes["NumLivroRegistro"].ToString();
                    dependente.NumFolhaRegistro = drDependentes["NumFolhaRegistro"].ToString();
                    dependente.IncideIRRF = Convert.ToInt32(drDependentes["IncideIRRF"]);
                    dependente.GrauParentesco = buscarGrauParentesco(drDependentes["GrauParentesco"].ToString());
                    dependente.DtEntregaCertNascimento = Convert.ToDateTime(drDependentes["DtEntregaCert"]);
                    dependente.IncideSalFamilia = drDependentes["IncideSalFamilia"].ToString();

                    dependentes.Add(dependente);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração: Chapa {0}, Nome Dependente: {1}. Motivo:{2}", dependente.Chapa, dependente.Nome, ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;
        }

        private string buscarGrauParentesco(string codGrau)
        {
            if (String.IsNullOrEmpty(codGrau))
                throw new BusinessException("Grau de parentesco não preenchido.");

            switch (codGrau)
            {
                case "Filho(a)":
                    return "1";
                case "Cônjuge":
                    return "5";
                case "Pai":
                    return "6";
                case "Mãe":
                    return "7";
                case "Sobrinho(a)":
                    return "9";
                case "Neto(a)":
                    return "9";
                case "Genro/Nora":
                    return "9";
                case "Enteado(a)":
                    return "3";
                case "Outros":
                    return "9";

                default:
                    return "";
            }
        }

        private string buscarEstadoCivil(string from)
        {
            switch (from)
            {
                case "Solteiro":
                    return EstadoCivil.Solteiro;
                case "Casado":
                    return EstadoCivil.Casado;
                case "Divorciado":
                    return EstadoCivil.Divorciado;
                case "Viúvo":
                    return EstadoCivil.Viuvo;
                case "Concubinato":
                    return EstadoCivil.Solteiro;
                case "Separado":
                    return EstadoCivil.Desquitado;
                case "União Estável":
                    return EstadoCivil.Solteiro;
                default:
                    return "";
            }
        }
    }
}
