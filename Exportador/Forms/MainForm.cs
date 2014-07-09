using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FileHelpers.DataLink;
using System.Configuration;
using System.Data.Common;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Exportador.RH.Ferias;
using Exportador.Academico;
using Exportador.RH.Horario;
using Exportador.RH.Funcionario;
using Exportador.BackOffice.ClienteFornecedor;
using Exportador.RH.Cargo;
using Exportador.RH.Ferias;
using Exportador.BackOffice.CentroCusto;
using Exportador.Lancamentos;
using Exportador.BackOffice.PlanoContas;
using Exportador.RH.Secao;
using Exportador.Interface;
using Exportador.RH.Historicos;
using Exportador.RH.PeriodoFerias;
using Exportador.RH.Dependente;
using Exportador.Academico.Habilitacao;
using Exportador.Academico.Curso;
using Exportador.Academico.PeriodoLetivo;
using Exportador.Academico.Aluno;
using Exportador.Academico.Pessoa;
using Exportador.Log;
using Exportador.Academico.Turno;
using Exportador.Academico.Horario;
using Exportador.Academico.Professor;
using Exportador.Academico.Disciplina;
using Exportador.Academico.MatrizCurricular.Grade;
using Exportador.Academico.Documento;
using Exportador.Academico.DocumentoAluno;
using Exportador.Academico.MatrizCurricular.Periodo;
using Exportador.Academico.MatrizCurricular.DisciplinaGrade;
using Exportador.Academico.Matricula.HabilitacaoAluno;
using Exportador.Academico.MatrizAplicada;
using Exportador.Academico.Turma.Turma;
using System.Diagnostics;
using Exportador.Academico.ParamCurso;
using Exportador.Academico.PessoaFiltro;
using Exportador.RH.Globais;

namespace FileHelpers
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region BackgroundWorkerEvents

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<IExportador> filaExportacao = (List<IExportador>)e.Argument;

            foreach (IExportador exporter in filaExportacao)
            {
                exporter.Exportar();

                System.Threading.Thread.Sleep(2000);//espera 2 segundos
            }            
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                string logMsg = String.Format("[{0}]:{1}", DateTime.Now.ToString(), e.UserState.ToString());

                Logger.Instance.Trace(txtPasta.Text, logMsg);

                txtStatus.AppendText((txtStatus.Text.Length > 0 ? System.Environment.NewLine : String.Empty) + logMsg);

            }

            progressBar1.Value = e.ProgressPercentage;

            lblPercentage.Text = String.Format("{0}% concluído.", e.ProgressPercentage.ToString());
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Concluído.");

            this.Cursor = Cursors.Arrow;
        }

        #endregion

        private IExportador ExportarClienteFornecedor()
        {
            criarDiretorio();

            string arquivoLancamentos = Path.Combine(txtPasta.Text, "clienteFornecedor.txt");

            ExportadorClienteFornecedor exporter = new ExportadorClienteFornecedor(arquivoLancamentos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            
            return exporter;
        }

        private IExportador ExportarLancamentos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "lancamentos.txt");

            ExportadorLancamentos exporter = new ExportadorLancamentos(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            
            return exporter;
        }

        private void criarDiretorio()
        {
            if (!Directory.Exists(txtPasta.Text))
            {
                Directory.CreateDirectory(txtPasta.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openDialog = new FolderBrowserDialog();
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtPasta.Text = openDialog.SelectedPath;
            {
            }
        }

        private void chkTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTodos.Checked)
            {
                txtRetornar.Text = "0";
                txtRetornar.Enabled = false;
            }
            else
            {
                txtRetornar.Enabled = true;
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            int tabela = listPossiveis.SelectedIndex;
            listExportar.Items.Add(listPossiveis.Items[tabela]);
            listPossiveis.Items.RemoveAt(tabela);
        }

        private void btnRemoverLista_Click(object sender, EventArgs e)
        {
            int tabela = listExportar.SelectedIndex;
            listPossiveis.Items.Add(listExportar.Items[tabela]);
            listExportar.Items.RemoveAt(tabela);
        }

        private void btnExportar_Click_1(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            
            List<IExportador> filaExportacao = new List<IExportador>();

            if (listExportar.Items.Count > 0)
            {
                foreach (var tabela in listExportar.Items)
                {
                    switch (tabela.ToString())
                    {
                        case "Acadêmico.Curso":
                            filaExportacao.Add(ExportarCursos());
                            break;
                        case "Acadêmico.Disciplina":
                            filaExportacao.Add(ExportarDisciplinas());
                            break;
                        case "Acadêmico.Documentos":
                            filaExportacao.Add(ExportarDocumentos());
                            break;
                        case "Acadêmico.DocumentosAluno":
                            filaExportacao.Add(ExportarDocumentosAluno());
                            break;
                        case "Acadêmico.SPLetivo":
                            filaExportacao.Add(ExportarPeriodosLetivos());
                            break;
                        case "Acadêmico.STurno":
                            filaExportacao.Add(ExportarTurnos());
                            break;

                        case "Acadêmico.Turmas.Turmas":
                            filaExportacao.Add(ExportarTurmas());
                            break;

                        case "Acadêmico.ParamsPorCurso.SHabilitacaoFilialPL":
                            filaExportacao.Add(ExportarParamsPorCurso());
                            break;                            
                        case "Academico.Matricula.HabilitacaoAluno":
                            filaExportacao.Add(ExportarHabilitacaoAluno());
                            break;                            
                        case "Academico.MatrizAplicada.HabilitacaoFilial":
                            filaExportacao.Add(ExportarHabilitacaoFilial());
                            break;                        
                        case "Acadêmico.MatrizCurricular.DisciplinaGrade":
                            filaExportacao.Add(ExportarDisciplinasGrades());
                            break;                        
                        case "Acadêmico.MatrizCurricular.Grade":
                            filaExportacao.Add(ExportarGrades());
                            break;
                        case "Acadêmico.MatrizCurricular.Período":
                            filaExportacao.Add(ExportarPeriodos());
                            break;
                        case "Acadêmico.SHabilitacao":
                            filaExportacao.Add(ExportarHabilitacoes());
                            break;
                        case "Acadêmico.SHorario":
                            filaExportacao.Add(ExportarHorario());
                            break;
                        case "Acadêmico.SAluno":
                            filaExportacao.Add(ExportarAlunos());
                            break;
                        case "Acadêmico.PPessoa":
                            filaExportacao.Add(ExportarPessoas());
                            break;
                        case "Acadêmico.Professor":
                            filaExportacao.Add(ExportarProfessor());
                            break;
                        case "BackOffice.CentroCusto":
                            filaExportacao.Add(ExportarCentroCusto());
                            break;
                        case "BackOffice.LançamentosBaixas":
                            filaExportacao.Add(ExportarLancamentos());
                            break;
                        case "BackOffice.PlanoContas":
                            filaExportacao.Add(ExportarPlanoContas());
                            break;
                        case "BackOffice.ClienteFornecedor":
                            filaExportacao.Add(ExportarClienteFornecedor());
                            break;
                        case "RH.Cargos":
                            filaExportacao.Add(ExportarCargos());
                            break;
                        case "RH.Funcionários":
                            filaExportacao.Add(ExportarFuncionarios());
                            break;
                        case "RH.Seções":
                            filaExportacao.Add(ExportarSecoes());
                            break;
                        case "RH.Funções":
                            filaExportacao.Add(ExportarFuncoes());
                            break;
                        case "RH.Horários":                            
                            filaExportacao.Add(ExportarHorarios());
                            break;
                        case "RH.Históricos.Afastamentos":
                            filaExportacao.Add(ExportarHistAfastamentos());
                            break;
                        case "RH.Históricos.Situações":
                            filaExportacao.Add(ExportarHistSituacoes());
                            break;
                        case "RH.Históricos.Funções":
                            filaExportacao.Add(ExportarHistFuncoes());
                            break;
                        case "RH.PeriodosFerias":
                            filaExportacao.Add(ExportarPeriodosFerias());
                            break;
                        case "RH.Históricos.Salários":
                            filaExportacao.Add(ExportarHistSalarios());
                            break;
                        case "RH.Históricos.Horários":
                            filaExportacao.Add(ExportarHistHorarios());
                            break;
                        case "RH.Históricos.Seções":
                            filaExportacao.Add(ExportarHistSecoes());
                            break;
                        case "RH.Dependentes":
                            filaExportacao.Add(ExportarDependentes());
                            break;
                        case "RH.FichaFinanceira":
                            filaExportacao.Add(ExportarFichaFinanceira());
                            break;
                        case "RH.Históricos.ContribuiçõesSindicais":
                            filaExportacao.Add(ExportarContribuicoesSindicais());
                            break;
                        case "RH.Históricos.OcorrênciasSEFIP":
                            filaExportacao.Add(ExportarHistOcorrenciasSEFIP());
                            break;
                        case "RH.Históricos.Endereços":
                            filaExportacao.Add(ExportarHistEnderecos());
                            break;
                        case "Acadêmico.PPessoaFiltro":
                            filaExportacao.Add(ExportarPessoaFiltro());
                            break;
                        case "RH.Férias.AquisicaoFerias":
                            filaExportacao.Add(ExportarAquisicaoFerias());
                            break;
                        case "RH.Férias.PeriodosGozo":
                            filaExportacao.Add(ExportarPeriodosGozo());
                            break;
                        case "RH.Férias.VerbasFerias":
                            filaExportacao.Add(ExportarVerbasFerias());
                            break;

                        case "RH.Férias.ReciboFerias":
                            filaExportacao.Add(ExportarReciboFerias());
                            break;

                        case "RH.ValoresPagosPensaoDependentes":
                            filaExportacao.Add(ExportarValoresPagosPensaoDependentes());
                            break;

                        case "RH.Históricos.DadosBancarios":
                            filaExportacao.Add(ExportarDadosBancarios());
                            break;

                        case "RH.Históricos.NumeroDependentes":
                            filaExportacao.Add(ExportarNumeroDependentes());
                            break;

                        case "RH.PeriodoDeFichaFinanceira":                            
                            filaExportacao.Add(ExportarPeriodoFichaFinanceira());
                            break;

                        case "RH.Bancos":
                            filaExportacao.Add(ExportarBancos());
                            break;

                        case "RH.Agências":
                            filaExportacao.Add(ExportarAgencias());
                            break;
                    }
                }

                bgWorker.RunWorkerAsync(filaExportacao);                    
            }
            else
            {
                MessageBox.Show("Não foram selecionadas tabelas para exportar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Cursor = Cursors.Arrow;
            }
        }

        private IExportador ExportarAgencias()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "Agencias.txt");

            ExportadorAgencias exporter = new ExportadorAgencias(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarBancos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "Bancos.txt");

            ExportadorBancos exporter = new ExportadorBancos(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarPeriodoFichaFinanceira()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "PeriodoFichaFinanceira.txt");

            ExportadorPeriodoFichaFinanceira exporter = new ExportadorPeriodoFichaFinanceira(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarNumeroDependentes()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "NumeroDependentes.txt");

            ExportadorNumeroDependentes exporter = new ExportadorNumeroDependentes(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarDadosBancarios()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "DadosBancarios.txt");

            ExportadorDadosBancarios exporter = new ExportadorDadosBancarios(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarValoresPagosPensaoDependentes()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "ValoresPagosPensaoDependentes.txt");

            ExportadorValoresPagosPensaoDependentes exporter = new ExportadorValoresPagosPensaoDependentes(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarReciboFerias()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "ReciboFerias.txt");

            ExportadorReciboFerias exporter = new ExportadorReciboFerias(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarVerbasFerias()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "VerbasFerias.txt");

            ExportadorVerbasFerias exporter = new ExportadorVerbasFerias(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarPeriodosGozo()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "PeriodosGozo.txt");

            ExportadorPeriodosGozo exporter = new ExportadorPeriodosGozo(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarAquisicaoFerias()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "AquisicaoFerias.txt");

            ExportadorAquisicaoFerias exporter = new ExportadorAquisicaoFerias(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarParamsPorCurso()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SHABILITACAOFILIALPL.txt");

            ExportadorParamsPorCurso exporter = new ExportadorParamsPorCurso(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarPessoaFiltro()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SPESSOAFILTRO.txt");

            ExportadorPessoaFiltro exporter = new ExportadorPessoaFiltro(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarTurmas()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "STURMA.txt");

            ExportadorTurma exporter = new ExportadorTurma(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarHabilitacaoFilial()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SHABILITACAOFILIAL.txt");

            ExportadorHabilitacaoFilial exporter = new ExportadorHabilitacaoFilial(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarHabilitacaoAluno()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SHABILITACAOALUNO.txt");

            ExportadorHabilitacaoAluno exporter = new ExportadorHabilitacaoAluno(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            this.Cursor = Cursors.WaitCursor;

            return exporter;
        }

        private IExportador ExportarDisciplinasGrades()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SDISCGRADE.txt");

            ExportadorDisciplinaGrade exporter = new ExportadorDisciplinaGrade(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarPeriodos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SPERIODO.txt");

            ExportadorPeriodo exporter = new ExportadorPeriodo(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarDocumentosAluno()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SDOCALUNO.txt");

            ExportadorDocAluno exporter = new ExportadorDocAluno(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarDocumentos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SDOCUMENTO.txt");

            ExportadorDocumento exporter = new ExportadorDocumento(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarGrades()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SGRADE.txt");

            ExportadorGrade exporter = new ExportadorGrade(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarDisciplinas()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SDISCIPLINA.txt");

            ExportadorDisciplina exporter = new ExportadorDisciplina(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarProfessor()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SPROFESSOR.txt");

            ExportadorProfessor exporter = new ExportadorProfessor(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHorario()
        {
            criarDiretorio();

            string arquivoLancamentos = Path.Combine(txtPasta.Text, "SHORARIO.txt");

            ExportadorHorario exporter = new ExportadorHorario(arquivoLancamentos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarTurnos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "STURNO.txt");

            ExportadorTurno exporter = new ExportadorTurno(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarPessoas()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "PPESSOA.txt");

            ExportadorPessoa exporter = new ExportadorPessoa(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarAlunos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SALUNO.txt");

            ExportadorSAluno exporter = new ExportadorSAluno(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarPeriodosLetivos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SPLETIVO.txt");

            ExportadorPeriodoLetivo exporter = new ExportadorPeriodoLetivo(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarCursos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "SCURSO.txt");

            ExportadorCurso exporter = new ExportadorCurso(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistEnderecos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histEnderecos.txt");

            ExportadorHistEnderecos exporter = new ExportadorHistEnderecos(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistOcorrenciasSEFIP()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histOcorrSEFIP.txt");

            ExportadorOcorrenciaSEFIP exporter = new ExportadorOcorrenciaSEFIP(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarContribuicoesSindicais()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "contrSindicais.txt");

            ExportadorContribuicaoSindical exporter = new ExportadorContribuicaoSindical(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarFichaFinanceira()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "fichaFinanceira.txt");

            ExportadorFichaFinanceira exporter = new ExportadorFichaFinanceira(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarDependentes()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "dependentes.txt");

            ExportadorDependente exporter = new ExportadorDependente(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistSecoes()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histSecoes.txt");

            ExportadorHistSecoes exporter = new ExportadorHistSecoes(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistHorarios()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histHorarios.txt");

            ExportadorHistHorarios exporter = new ExportadorHistHorarios(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistSalarios()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histSalarios.txt");

            ExportadorSalarios exporter = new ExportadorSalarios(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarPeriodosFerias()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "periodosFerias.txt");

            ExportadorPeriodoFerias exporter = new ExportadorPeriodoFerias(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistFuncoes()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histFuncoes.txt");

            ExportadorFuncoes exporter = new ExportadorFuncoes(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistSituacoes()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histSituacoes.txt");

            ExportadorSituacoes exporter = new ExportadorSituacoes(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHistAfastamentos()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "histAfastamentos.txt");

            ExportadorAfastamentos exporter = new ExportadorAfastamentos(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarFuncionarios()
        {
            string fileName = Path.Combine(txtPasta.Text, "funcionarios.txt");

            ExportadorFuncionario exporter = new ExportadorFuncionario(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            bgWorker.RunWorkerCompleted -= bgWorker_RunWorkerCompleted;

            return exporter;
        }

        private IExportador ExportarHorarios()
        {
            criarDiretorio();

            string arquivoLancamentos = Path.Combine(txtPasta.Text, "horarios.txt");

            ExportadorHorarios exporter = new ExportadorHorarios(arquivoLancamentos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarHabilitacoes()
        {
            criarDiretorio();

            string arquivoLancamentos = Path.Combine(txtPasta.Text, "shabilitacao.txt");

            ExportadorSHabilitacao exporter = new ExportadorSHabilitacao(arquivoLancamentos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarFuncoes()
        {
            criarDiretorio();

            string arquivoLancamentos = Path.Combine(txtPasta.Text, "funcoes.txt");

            ExportadorFuncao exporter = new ExportadorFuncao(arquivoLancamentos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarSecoes()
        {
            criarDiretorio();

            string arquivoLancamentos = Path.Combine(txtPasta.Text, "secoes.txt");

            ExportadorSecao exporter = new ExportadorSecao(arquivoLancamentos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarPlanoContas()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "planoContas.txt");

            ExportadorPlanoContas exporter = new ExportadorPlanoContas(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarCargos()
        {
            criarDiretorio();

            string arquivoCargos = Path.Combine(txtPasta.Text, "cargos.txt");

            ExportadorCargo exporter = new ExportadorCargo(arquivoCargos, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private IExportador ExportarCentroCusto()
        {
            criarDiretorio();

            string fileName = Path.Combine(txtPasta.Text, "centroCusto.txt");

            ExportadorCentroCusto exporter = new ExportadorCentroCusto(fileName, bgWorker);
            exporter.PageSize = Convert.ToInt32(string.IsNullOrEmpty(txtPageSize.Text) ? "0" : txtPageSize.Text);
            exporter.RecordsCount = Convert.ToInt32(txtRetornar.Text);
            exporter.DebugMode = chkDebug.Checked;

            return exporter;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(txtPasta.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IExportador exporter = ExportarHorarios();
            exporter.Exportar();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<IExportador> filaExportacao = new List<IExportador>();
            
            filaExportacao.Add(ExportarPessoaFiltro());
            
            bgWorker.RunWorkerAsync(filaExportacao);  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<IExportador> filaExportacao = new List<IExportador>();

            filaExportacao.Add(ExportarHistAfastamentos());

            bgWorker.RunWorkerAsync(filaExportacao);   
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            List<IExportador> filaExportacao = new List<IExportador>();

            filaExportacao.Add(ExportarHistEnderecos());

            bgWorker.RunWorkerAsync(filaExportacao);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            List<IExportador> filaExportacao = new List<IExportador>();

            filaExportacao.Add(ExportarProfessor());

            bgWorker.RunWorkerAsync(filaExportacao);
        }

    }
}
