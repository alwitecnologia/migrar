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

namespace Exportador.Academico.Curso
{
    public class ExportadorCurso: IExportador
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
        public ExportadorCurso()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorCurso(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorCurso(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private static string _queryCursos = @"SELECT DISTINCT
                                            C.ID as CODCURSO,
                                            IIF(C.NOME_OFICIAL IS NULL,C.NOME,C.NOME_OFICIAL) as NOME,
                                            C.NOME as COMPLEMENTO,
                                            TRIM(C.LEI_AUTORIZACAO) as DECRETO,
                                            C.HABILITACAO as HABILITACAO,
                                            C.ID_AREA_DO_CONHECIMENTO as IDAREA,
                                            NIVEL_CURSO.nome AS TipoCurso,
                                            AREA.NOME AS NOMEAREA
                                        FROM CURSO C
                                        INNER JOIN MATRICULA_CURSO MC ON MC.ID_CURSO=C.ID
                                        INNER JOIN NIVEL_CURSO ON NIVEL_CURSO.ID=C.ID_NIVEL_CURSO
                                        INNER JOIN AREA_DO_CONHECIMENTO AS AREA ON AREA.ID=C.ID_AREA_DO_CONHECIMENTO
                                        WHERE C.ID NOT IN (0,10)";

        private static string _queryCountCursos = @"SELECT COUNT(*)
                                            FROM (" + _queryCursos + ") AS COUNTER";

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

            List<Curso> cursos = new List<Curso>();

            error = buscarCursos(cursos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Curso), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, cursos);
        }

        private double getCount()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            object count;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountCursos))
            {
                count = database.ExecuteScalar(command);
            }

            return Convert.ToDouble(count);
        }

        private bool buscarCursos(List<Curso> cursos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand command = database.GetSqlStringCommand(_queryCursos);

            double totalRecords = getCount();

            IDataReader drCursos = database.ExecuteReader(command);

            double processedRecords = 0;

            while (drCursos.Read())
            {
                Curso curso = new Curso();

                try
                {
                    processedRecords++;

                    curso.CodTipoCurso = _cursoDAO.buscarTipoCurso((string)drCursos["TipoCurso"], (string)drCursos["NOME"]);
                    curso.CodCurso = drCursos["CODCURSO"].ToString();
                    curso.Nome = drCursos["NOME"].ToString().RemoveSpecialChars();
                    curso.Complemento = drCursos["Complemento"].ToString().RemoveSpecialChars();
                    curso.Decreto = drCursos["DECRETO"].ToString().RemoveSpecialChars();
                    curso.Area = buscarAreaConhecimento(drCursos["IDAREA"].ToString(), drCursos["NOMEAREA"].ToString()).RemoveSpecialChars();
                    
                    curso.CodColigada = 1;
                    curso.Habilitacao = curso.Nome;

                    cursos.Add(curso);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o curso: Código {0},Nome:{1},Motivo:{2}", curso.CodCurso,curso.Nome, ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }

        private string buscarAreaConhecimento(string idArea,string nomeArea)
        {
            string area = "";

            switch (idArea)
            {
                case "3":
                case "7":
                case "8":
                    area = "Ciências Humanas";
                    break;
                case "2":
                case "30":
                    area = "Ciências Biológicas e da Saúde";
                    break;
                case "4":
                case "6":
                    area = "Ciências Sociais Aplicadas";
                    break;
                case "1":
                case "27":
                    area = "Ciências Exatas e Tecnológicas";
                    break;
            }

    //WHEN AC.ID IN (3,7,8) THEN 'Ciências Humanas'
    //WHEN AC.ID IN (2,30) THEN 'Ciências Biológicas e da Saúde'
    //WHEN AC.ID IN (4,6) OR C.ID IN (18,542,563) THEN 'Ciências Sociais Aplicadas'
    //WHEN AC.ID IN (1,27) OR C.ID = 58 THEN 'Ciências Exatas e Tecnológicas'

            if (String.IsNullOrEmpty(area))
                throw new BusinessException(String.Format("Curso não possui área de conhecimento relacionada no sistema de origem.Id:{1} Nome:{0}", nomeArea,idArea));

            return area;
        }

    }
}
