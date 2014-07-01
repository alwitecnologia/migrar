using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.DAO;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.Helpers;

namespace Exportador.Academico.Disciplina
{
    public class ExportadorDisciplina : IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsCount;
        private bool _debugMode;
        private bool _error = false;
        private List<Curso.Curso> cursos = new List<Curso.Curso>();

        #endregion

        #region Properties

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int RecordsCount
        {
            get { return _recordsCount; }
            set { _recordsCount = value; }
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
        public ExportadorDisciplina()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorDisciplina(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorDisciplina(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryGradesDisciplina = @"SELECT DISTINCT GD.ID_GRADE,GD.ID_NOME_DISCIPLINA
                                                    FROM GRADE_DISCIPLINA GD
                                                    INNER JOIN MATRICULA_DISCIPLINA MD ON MD.ID_GRADE_DISCIPLINA=GD.ID
                                                    ORDER BY ID_GRADE,ID_NOME_DISCIPLINA";

        private string _queryGrades = @"SELECT ID,ID_CURSO FROM GRADE
                                        WHERE ID IS NOT NULL
                                            AND ID_CURSO IS NOT NULL";


        private string _buscarTodasDisciplinas = @"select g.id_curso as cod_curso
                                                        ,gd.id as cod_disciplina
                                                        ,nd.nome as nome_disciplina
                                                        ,ta.nota_conceito
                                                    from grade_disciplina gd
                                                    inner join grade g on gd.id_grade=g.id
                                                    inner join nome_disciplina nd on gd.id_nome_disciplina=nd.id
                                                    inner join tipo_avaliacao ta on gd.id_tipo_avaliacao=ta.id";

        private string _countDisciplinas = @"SELECT COUNT(*)
                                            FROM (

                                                    select g.id_curso as cod_curso
                                                        ,gd.id as cod_disciplina
                                                        ,nd.nome as nome_disciplina
                                                        ,ta.nota_conceito
                                                    from grade_disciplina gd
                                                    inner join grade g on gd.id_grade=g.id
                                                    inner join nome_disciplina nd on gd.id_nome_disciplina=nd.id
                                                    inner join tipo_avaliacao ta on gd.id_tipo_avaliacao=ta.id

                                            ) AS CT";

        #endregion

        public void Exportar()
        {
            try
            {
                _bgWorker.ReportProgress(0, "Buscando cursos já cadastrados...");

                cursos = (new CursoDAO()).buscarTodosDestino();

                _bgWorker.ReportProgress(60, "Buscando disciplinas no sistema origem...");

                List<Disciplina> disciplinas = BuscarDisciplinas();

                _bgWorker.ReportProgress(80, "Buscando disciplinas no sistema origem...");
                                
                FileHelperEngine engine = new FileHelperEngine(typeof(Disciplina), Encoding.Unicode);

                _bgWorker.RunWorkerCompleted += workerCompleted;

                engine.WriteFile(_filename, disciplinas);

                _bgWorker.ReportProgress(100);
            }
            catch (Exception e)
            {
                _error = true;

                _bgWorker.ReportProgress(0, e.Message + System.Environment.NewLine + e.StackTrace);
            }
        }

        /// <summary>
        /// Retorna todas as disciplinas cadastradas no sistema de origem.
        /// </summary>
        /// <returns></returns>
        private List<Disciplina> BuscarDisciplinas()
        {
            List<Disciplina> lDisc = new List<Disciplina>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            _bgWorker.ReportProgress(0, "Gerando contadores...");

            double totalRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_countDisciplinas))
            {
                totalRecords = (int)database.ExecuteScalar(command);
            }

            using (DbCommand command = database.GetSqlStringCommand(_buscarTodasDisciplinas))
            {
                var reader = database.ExecuteReader(command);

                _bgWorker.ReportProgress(0, "Executando consulta na base de origem...");

                double processedRecords = 0;

                _bgWorker.ReportProgress(0, "Gerando registros de destino...");

                while (reader.Read())
                {
                    try
                    {
                        lDisc.Add(ConverterDisciplina(reader));

                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string codDisc = reader.GetString("cod_disciplina");

                        string msg = String.Format("Não foi possível exportar disciplina código {0}. Motivo:{1}", codDisc, ex.Message);

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100),msg);
                    }
                }
            }

            return lDisc;
        }

        private Disciplina ConverterDisciplina(IDataReader reader)
        {
            Disciplina disc = new Disciplina();

            //g.id_curso as cod_curso
            //,gd.id as cod_disciplina
            //,nd.nome as nome_disciplina
            //,ta.nota_conceito

            disc.CodDisc = reader.GetString("cod_disciplina");
            disc.Nome = reader.GetString("nome_disciplina").Replace(";","- ");
            disc.TipoNota = reader.GetString("nota_conceito");

            string codCurso = reader.GetString("cod_curso");

            if (cursos.Exists(c => c.CodCurso == codCurso))
                disc.CodTipoCurso = cursos.Where(c => c.CodCurso == codCurso).First().CodTipoCurso;
            else
                throw new BusinessException(String.Format("Curso código {0} não encontrado.",codCurso));

            disc.CodColigada = 1;
            disc.CursoLivre = "N";
            disc.TipoAula = "M";


            return disc;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_error)
            {
                MessageBox.Show("Houveram erros no processo.", "Erro.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Processo concluído com sucesso.", "Sucesso.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

}
