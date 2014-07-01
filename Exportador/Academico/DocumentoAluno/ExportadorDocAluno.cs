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

namespace Exportador.Academico.DocumentoAluno
{
    public class ExportadorDocAluno: IExportador
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
        public ExportadorDocAluno()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorDocAluno(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorDocAluno(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodosDocumentosDosAlunos = @"

select first 200 * from (

select distinct
                                                            curso.nome as nomecurso
                                                            ,nivel_curso.nome as tipocurso
                                                            ,coalesce(doc_aluno.data_entrega,cast(doc_aluno.dua as date)) as dataentrega
                                                            ,u_aluno.matricula
                                                            ,doc_aluno.id_documento
                                                            ,documento.nome as nomedoc
                                                        from documento_matricula doc_aluno
                                                        inner join documento on documento.id=doc_aluno.id_documento
                                                        inner join curso on curso.id=doc_aluno.id_curso_original
                                                        inner join nivel_curso on nivel_curso.id=curso.id_nivel_curso
                                                        inner join usuario u_aluno on u_aluno.id=doc_aluno.id_aluno


) as docaluno
";

        private string _queryCountDocsAlunos = @"SELECT COUNT(*) CT FROM (

                                                        select distinct
                                                            curso.nome as nomecurso
                                                            ,nivel_curso.nome as tipocurso
                                                            ,coalesce(doc_aluno.data_entrega,cast(doc_aluno.dua as date)) as dataentrega
                                                            ,u_aluno.matricula
                                                            ,documento.nome as nomedoc
                                                        from documento_matricula doc_aluno
                                                        inner join documento on documento.id=doc_aluno.id_documento
                                                        inner join curso on curso.id=doc_aluno.id_curso_original
                                                        inner join nivel_curso on nivel_curso.id=curso.id_nivel_curso
                                                        inner join usuario u_aluno on u_aluno.id=doc_aluno.id_aluno

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

            List<DocumentoAluno> docs = new List<DocumentoAluno>();

            docs = buscarDocsAlunos();

            FileHelperEngine engine = new FileHelperEngine(typeof(DocumentoAluno), Encoding.Unicode);
            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, docs);
        }

        private List<DocumentoAluno> buscarDocsAlunos()
        {
            List<DocumentoAluno> lDocsAlunos = new List<DocumentoAluno>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountDocsAlunos))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodosDocumentosDosAlunos))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        lDocsAlunos.Add(ConverterDocAluno(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string idDoc = (reader["id_documento"] == DBNull.Value) ? String.Empty : reader["id_documento"].ToString();
                        string matAluno = (reader["matricula"] == DBNull.Value) ? String.Empty : reader["matricula"].ToString();

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o documento do aluno: Código {0}, Aluno: {1},Motivo:{1}", idDoc, matAluno, ex.Message));
                    }
                }
            }

            return lDocsAlunos;
        }

        private DocumentoAluno ConverterDocAluno(IDataReader drDoc)
        {
            DocumentoAluno d = new DocumentoAluno();
                                   
            string tipoCurso = (drDoc["tipocurso"] == DBNull.Value) ? String.Empty : drDoc["tipocurso"].ToString();
            string nomeCurso = (drDoc["nomecurso"] == DBNull.Value) ? String.Empty : drDoc["nomecurso"].ToString();

            d.CodTipoCurso = (new CursoDAO()).buscarTipoCurso(tipoCurso, nomeCurso);

            DateTime dtEntrega = Convert.ToDateTime(drDoc["dataentrega"]);

            string semestreEntrega = ((dtEntrega.Month>6?2:1)).ToString();
            string anoEntrega = dtEntrega.Year.ToString();

            d.CodPerLet = String.Format("{0}/{1}", anoEntrega, semestreEntrega);

            d.RA = (drDoc["matricula"] == DBNull.Value) ? String.Empty : drDoc["matricula"].ToString();
            //d.DescDocumento = (drDoc["nomedoc"] == DBNull.Value) ? String.Empty : drDoc["nomedoc"].ToString();


            string descDoc = (drDoc["nomedoc"] == DBNull.Value) ? String.Empty : drDoc["nomedoc"].ToString();
            d.DescDocumento = (descDoc.Length > 57) ? String.Concat(descDoc.Substring(0, 57), "...") : descDoc;

            d.DtEntrega = dtEntrega;

            d.CodColigada = 1;
            d.CodFilial = 1;
            
            return d;
        }


        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodosDocumentosDosAlunos);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }
    }
}
