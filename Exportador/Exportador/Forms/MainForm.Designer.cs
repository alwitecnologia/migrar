namespace FileHelpers
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.txtPageSize = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.chkTodos = new System.Windows.Forms.CheckBox();
            this.txtRetornar = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPasta = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnRemoverLista = new System.Windows.Forms.Button();
            this.btnListaExportar = new System.Windows.Forms.Button();
            this.listExportar = new System.Windows.Forms.ListBox();
            this.listPossiveis = new System.Windows.Forms.ListBox();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(12, 314);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(666, 135);
            this.txtStatus.TabIndex = 1;
            this.txtStatus.Text = "";
            this.txtStatus.WordWrap = false;
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // txtPageSize
            // 
            this.txtPageSize.Location = new System.Drawing.Point(123, 15);
            this.txtPageSize.Name = "txtPageSize";
            this.txtPageSize.Size = new System.Drawing.Size(77, 20);
            this.txtPageSize.TabIndex = 5;
            this.txtPageSize.Text = "7500";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.chkDebug);
            this.groupBox1.Controls.Add(this.chkTodos);
            this.groupBox1.Controls.Add(this.txtRetornar);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPasta);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPageSize);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(665, 106);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configurações";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(217, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(190, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "Visualizar arquivos na pasta";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(423, 15);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(87, 17);
            this.chkDebug.TabIndex = 12;
            this.chkDebug.Text = "Debug mode";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // chkTodos
            // 
            this.chkTodos.AutoSize = true;
            this.chkTodos.Checked = true;
            this.chkTodos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTodos.Location = new System.Drawing.Point(541, 17);
            this.chkTodos.Name = "chkTodos";
            this.chkTodos.Size = new System.Drawing.Size(93, 17);
            this.chkTodos.TabIndex = 11;
            this.chkTodos.Text = "Retorna todos";
            this.chkTodos.UseVisualStyleBackColor = true;
            this.chkTodos.CheckedChanged += new System.EventHandler(this.chkTodos_CheckedChanged);
            // 
            // txtRetornar
            // 
            this.txtRetornar.Enabled = false;
            this.txtRetornar.Location = new System.Drawing.Point(320, 14);
            this.txtRetornar.Name = "txtRetornar";
            this.txtRetornar.Size = new System.Drawing.Size(87, 20);
            this.txtRetornar.TabIndex = 10;
            this.txtRetornar.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Registros a retornar:";
            // 
            // txtPasta
            // 
            this.txtPasta.Location = new System.Drawing.Point(123, 41);
            this.txtPasta.Name = "txtPasta";
            this.txtPasta.Size = new System.Drawing.Size(536, 20);
            this.txtPasta.TabIndex = 8;
            this.txtPasta.Text = "C:\\Users\\AL-WI3\\Desktop\\beppler\\lancamentos";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Local arquivos:";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Registros por ciclo:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 456);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(664, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExportar);
            this.groupBox2.Controls.Add(this.btnRemoverLista);
            this.groupBox2.Controls.Add(this.btnListaExportar);
            this.groupBox2.Controls.Add(this.listExportar);
            this.groupBox2.Controls.Add(this.listPossiveis);
            this.groupBox2.Location = new System.Drawing.Point(13, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(665, 169);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Exportar";
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(306, 118);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(75, 23);
            this.btnExportar.TabIndex = 17;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click_1);
            // 
            // btnRemoverLista
            // 
            this.btnRemoverLista.Location = new System.Drawing.Point(306, 69);
            this.btnRemoverLista.Name = "btnRemoverLista";
            this.btnRemoverLista.Size = new System.Drawing.Size(75, 23);
            this.btnRemoverLista.TabIndex = 16;
            this.btnRemoverLista.Text = "<<";
            this.btnRemoverLista.UseVisualStyleBackColor = true;
            this.btnRemoverLista.Click += new System.EventHandler(this.btnRemoverLista_Click);
            // 
            // btnListaExportar
            // 
            this.btnListaExportar.Location = new System.Drawing.Point(306, 40);
            this.btnListaExportar.Name = "btnListaExportar";
            this.btnListaExportar.Size = new System.Drawing.Size(75, 23);
            this.btnListaExportar.TabIndex = 15;
            this.btnListaExportar.Text = ">>";
            this.btnListaExportar.UseVisualStyleBackColor = true;
            this.btnListaExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // listExportar
            // 
            this.listExportar.FormattingEnabled = true;
            this.listExportar.Location = new System.Drawing.Point(393, 26);
            this.listExportar.Name = "listExportar";
            this.listExportar.Size = new System.Drawing.Size(266, 121);
            this.listExportar.Sorted = true;
            this.listExportar.TabIndex = 14;
            // 
            // listPossiveis
            // 
            this.listPossiveis.FormattingEnabled = true;
            this.listPossiveis.Items.AddRange(new object[] {
            "Acadêmico.Curso",
            "Acadêmico.Disciplina",
            "Acadêmico.Documentos",
            "Acadêmico.DocumentosAluno",
            "Academico.Matricula.HabilitacaoAluno",
            "Academico.MatrizAplicada.HabilitacaoFilial",
            "Acadêmico.MatrizCurricular.DisciplinaGrade",
            "Acadêmico.MatrizCurricular.Grade",
            "Acadêmico.MatrizCurricular.Período",
            "Acadêmico.ParamsPorCurso.SHabilitacaoFilialPL",
            "Acadêmico.PPessoa",
            "Acadêmico.Professor",
            "Acadêmico.SAluno",
            "Acadêmico.SHabilitacao",
            "Acadêmico.SHorario",
            "Acadêmico.SPLetivo",
            "Acadêmico.STurno",
            "Acadêmico.Turmas.Turmas",
            "BackOffice.CentroCusto",
            "BackOffice.ClienteFornecedor",
            "BackOffice.LançamentosBaixas",
            "BackOffice.PlanoContas",
            "RH.Cargos",
            "RH.Dependentes",
            "RH.FichaFinanceira",
            "RH.Funcionários",
            "RH.Funções",
            "RH.Históricos.Afastamentos",
            "RH.Históricos.ContribuiçõesSindicais",
            "RH.Históricos.Endereços",
            "RH.Históricos.Funções",
            "RH.Históricos.Horários",
            "RH.Históricos.OcorrênciasSEFIP",
            "RH.Históricos.Salários",
            "RH.Históricos.Seções",
            "RH.Históricos.Situações",
            "RH.Horários",
            "RH.PeriodosFerias",
            "RH.Seções"});
            this.listPossiveis.Location = new System.Drawing.Point(20, 26);
            this.listPossiveis.Name = "listPossiveis";
            this.listPossiveis.Size = new System.Drawing.Size(266, 121);
            this.listPossiveis.Sorted = true;
            this.listPossiveis.TabIndex = 13;
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(12, 439);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(0, 13);
            this.lblPercentage.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 498);
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exportação de Fornecedores e Clientes - Sapiens -> RM";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtStatus;
        public System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.TextBox txtPageSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPasta;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkTodos;
        private System.Windows.Forms.TextBox txtRetornar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listPossiveis;
        private System.Windows.Forms.ListBox listExportar;
        private System.Windows.Forms.Button btnRemoverLista;
        private System.Windows.Forms.Button btnListaExportar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.Button button2;
    }
}

