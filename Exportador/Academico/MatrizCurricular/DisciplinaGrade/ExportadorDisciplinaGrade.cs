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
using Exportador.Academico.Disciplina;
using Exportador.DAO;

namespace Exportador.Academico.MatrizCurricular.DisciplinaGrade
{
    public class ExportadorDisciplinaGrade: IExportador
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
        public ExportadorDisciplinaGrade()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorDisciplinaGrade(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorDisciplinaGrade(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodasDisciplinasGradeObrigatorias = @"select
                                                                        g.id_curso as cod_curso
                                                                        ,g.id as cod_grade
                                                                        ,gd.fase as cod_periodo
                                                                        ,gd.id as cod_disciplina
                                                                        ,nd.nome as desc_disciplina
                                                                        ,gd.carga_horaria
                                                                        ,ta.nota_conceito
                                                                    from grade_disciplina gd
                                                                    inner join grade g on gd.id_grade=g.id
                                                                    inner join nome_disciplina nd on gd.id_nome_disciplina=nd.id
                                                                    inner join tipo_avaliacao ta on gd.id_tipo_avaliacao=ta.id
                                                                where gd.fase > 0";

        private string _queryTodasDisciplinasGradeOptativas = @"SELECT DISTINCT
                                                                    GRADE.ID_CURSO AS CODCURSO
                                                                    ,GRADE.ID AS CODGRADE
                                                                    ,GD.FASE AS CODPERIODO
                                                                    ,GDO.ID_NOME_DISCIPLINA AS CODDISC
                                                                    ,ND_OPT.NOME AS DESCRICAO
                                                                    ,GDO.CARGA_HORARIA
                                                                FROM GRADE_DISCIPLINA GD
                                                                INNER JOIN GRADE ON GRADE.ID=GD.ID_GRADE
                                                                INNER JOIN NOME_DISCIPLINA ND ON ND.ID=GD.ID_NOME_DISCIPLINA
                                                                INNER JOIN GRADE_DISCIPLINA_OPTATIVA GDO ON GDO.ID_GRADE_DISCIPLINA=GD.ID
                                                                INNER JOIN NOME_DISCIPLINA ND_OPT ON ND_OPT.ID=GDO.ID_NOME_DISCIPLINA

                                                                ORDER BY ND_OPT.NOME";

        private string _queryCountObrigatorias = @"SELECT COUNT(*) CT FROM (
                                                                    select
                                                                        g.id_curso as cod_curso
                                                                        ,g.id as cod_grade
                                                                        ,gd.fase as cod_periodo
                                                                        ,gd.id as cod_disciplina
                                                                        ,nd.nome as desc_disciplina
                                                                        ,gd.carga_horaria
                                                                        ,ta.nota_conceito
                                                                    from grade_disciplina gd
                                                                    inner join grade g on gd.id_grade=g.id
                                                                    inner join nome_disciplina nd on gd.id_nome_disciplina=nd.id
                                                                    inner join tipo_avaliacao ta on gd.id_tipo_avaliacao=ta.id
                                            ) AS CT";

        private string _queryCountOptativas = @"SELECT COUNT(*) CT FROM (
                                                                SELECT DISTINCT
                                                                    GRADE.ID_CURSO AS CODCURSO
                                                                    ,GRADE.ID AS CODGRADE
                                                                    ,GD.FASE AS CODPERIODO
                                                                    ,GDO.ID_NOME_DISCIPLINA AS CODDISC
                                                                    ,ND_OPT.NOME AS DESCRICAO
                                                                    ,GDO.CARGA_HORARIA
                                                                FROM GRADE_DISCIPLINA GD
                                                                INNER JOIN GRADE ON GRADE.ID=GD.ID_GRADE
                                                                INNER JOIN NOME_DISCIPLINA ND ON ND.ID=GD.ID_NOME_DISCIPLINA

                                                                INNER JOIN GRADE_DISCIPLINA_OPTATIVA GDO ON GDO.ID_GRADE_DISCIPLINA=GD.ID
                                                                INNER JOIN NOME_DISCIPLINA ND_OPT ON ND_OPT.ID=GDO.ID_NOME_DISCIPLINA

                                                                ORDER BY ND_OPT.NOME
                                            ) AS CT";

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

            _bgWorker.ReportProgress(0, "Buscando cursos já cadastrados...");

            List<Curso.Curso> cursosRM = (new CursoDAO()).buscarTodosDestino();

            _bgWorker.ReportProgress(20, "Buscando grades já cadastradas...");

            List<Grade.Grade> gradesRM = (new GradeDAO()).buscarTodasDestino();

            _bgWorker.ReportProgress(40, "Buscando grades-disciplinas obrigatórias no sistema origem que possuem alunos matriculados...");
            
            List<DisciplinaGrade> disciplinasGrade = new List<DisciplinaGrade>();

            disciplinasGrade = buscarDisciplinasObgGrade();

            _bgWorker.ReportProgress(60,"Gerando registros para exportação...");

            removerDisciplinasNaoCadastradas(disciplinasGrade);
            
            //Por enquanto não serão buscados até se verificar como estão relacionadas estas disciplinas.
            //disciplinasGrade.AddRange(buscarDisciplinasOptGrade());

            //gerarPosicaoHistorico(disciplinasGrade);

            FileHelperEngine engine = new FileHelperEngine(typeof(DisciplinaGrade), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, disciplinasGrade);

            _bgWorker.ReportProgress(100);

        }

        private void removerDisciplinasNaoCadastradas(List<DisciplinaGrade> disciplinasGrade)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            List<string> disciplinasRM = new List<string>();

            using (DbCommand command = database.GetSqlStringCommand("SELECT CODDISC FROM SDISCIPLINA"))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    disciplinasRM.Add(reader.GetString("CODDISC"));
                }
            }

            List<DisciplinaGrade> discGradeFiltrado = new List<DisciplinaGrade>();

            foreach (var codDisc in disciplinasRM)
            {
                discGradeFiltrado.AddRange(disciplinasGrade.Where(dg => dg.CodDisc == codDisc));
            }

            disciplinasGrade.Clear();

            disciplinasGrade.AddRange(discGradeFiltrado);
        }

        /// <summary>
        /// Busca as disciplinas optativas/eletivas da grade.
        /// </summary>
        /// <returns></returns>
        private List<DisciplinaGrade> buscarDisciplinasOptGrade()
        {
            List<DisciplinaGrade> lDisciplinasGrade = new List<DisciplinaGrade>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountOptativas))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodasDisciplinasGradeOptativas))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        lDisciplinasGrade.Add(ConverterDisciplinaGrade(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a disciplina da grade: Motivo:{0}", ex.Message));
                    }
                }
            }

            return lDisciplinasGrade;
        }

        private void gerarPosicaoHistorico(List<DisciplinaGrade> disciplinasGrade)
        {
            var rows  = disciplinasGrade
                    .Select(dg => new {dg.CodCurso,dg.CodGrade,dg.CodPeriodo})
                    .Distinct()
                   .ToArray();

            foreach (var item in rows)
            {
                int rowNumber = 1;

                foreach (DisciplinaGrade dg in disciplinasGrade
                        .Where(x => x.CodCurso==item.CodCurso 
                            || x.CodGrade==item.CodGrade 
                            || x.CodPeriodo==item.CodPeriodo)
                        )
                {
                    dg.PosicaoNoHistorico = rowNumber;
                    rowNumber++;
                }
            }
        }

        /// <summary>
        /// Busca as disciplinas obrigatórias da grade.
        /// </summary>
        /// <returns></returns>
        private List<DisciplinaGrade> buscarDisciplinasObgGrade()
        {
            List<DisciplinaGrade> lDisciplinasGrade = new List<DisciplinaGrade>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountObrigatorias))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodasDisciplinasGradeObrigatorias))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        DisciplinaGrade dg = new DisciplinaGrade();

                        dg = ConverterDisciplinaGrade(reader);

                        dg.TipoDisciplina = "B";//Não colocado no método de converter, pois aqui são carregadas apenas as obrigatórias.

                        lDisciplinasGrade.Add(dg);
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a disciplina da grade: Motivo:{0}", ex.Message));
                    }
                }
            }

            return lDisciplinasGrade;
        }

        /// <summary>
        /// Busca as disciplinas optativas/eletivas da grade.
        /// </summary>
        /// <returns></returns>
        private List<DisciplinaGrade> buscarDisciplinasOptEletGrade()
        {
            List<DisciplinaGrade> lDisciplinasGrade = new List<DisciplinaGrade>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountObrigatorias))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodasDisciplinasGradeObrigatorias))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        DisciplinaGrade dg = new DisciplinaGrade();

                        dg = ConverterDisciplinaGrade(reader);
                        dg.TipoDisciplina = "E";

                        lDisciplinasGrade.Add(dg);
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a disciplina da grade: Motivo:{0}", ex.Message));
                    }
                }
            }

            return lDisciplinasGrade;
        }


        private DisciplinaGrade ConverterDisciplinaGrade(IDataReader drDisciplinaGrade)
        {
            DisciplinaGrade dg = new DisciplinaGrade();

            dg.CodGrade = drDisciplinaGrade.GetString("cod_grade");
            dg.CodPeriodo = drDisciplinaGrade.GetNullableInt32("cod_periodo");
            dg.CargaHoraria = drDisciplinaGrade.GetNullableInt32("carga_horaria");
            dg.CodDisc = drDisciplinaGrade.GetString("cod_disciplina");
            dg.TipoNota = drDisciplinaGrade.GetString("nota_conceito");
            dg.CodCurso = drDisciplinaGrade.GetString("cod_curso");
            dg.CodHabilitacao = drDisciplinaGrade.GetString("cod_curso");

            string desc = drDisciplinaGrade.GetString("desc_disciplina");

            dg.Descricao = desc.Substring(0, Math.Min(desc.Length, 60));           

            dg.CodColigada = 1;
            dg.NumCasasDecimaisNota = 1;
            dg.Atividade = "2";
            dg.CalcMediaGlobal = "N";
            dg.DesempenhoAluno = "N";
            dg.ImprimeBoletim = "S";            

            return dg;
        }


        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodasDisciplinasGradeObrigatorias);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }
    }


}
