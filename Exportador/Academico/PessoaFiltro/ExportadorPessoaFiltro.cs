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
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using Exportador.DAO;

namespace Exportador.Academico.PessoaFiltro
{
    public class ExportadorPessoaFiltro: IExportador
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
        public ExportadorPessoaFiltro()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPessoaFiltro(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPessoaFiltro(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryPessoaFiltro = @"SELECT codigo,
                                                    nome,
                                                    apelido,
                                                    dtnascimento,
                                                    estadocivil,
                                                    sexo,
                                                    naturalidade,
                                                    estadonatal,
                                                    nacionalidade,
                                                    grauinstrucao,
                                                    rua,
                                                    numero,
                                                    complemento,
                                                    bairro,
                                                    estado,
                                                    cidade,
                                                    cep,
                                                    pais,
                                                    regprofissional,
                                                    cpf,
                                                    telefone1,
                                                    telefone2,
                                                    telefone3,
                                                    fax,
                                                    email,
                                                    cartidentidade,
                                                    ufcartident,
                                                    orgemissorident,
                                                    dtemissaoident,
                                                    tituloeleitor,
                                                    zonatiteleitor,
                                                    secaotiteleitor ,
                                                    dttiteleitor ,
                                                    esteleit ,
                                                    carteiratrab,
                                                    seriecarttrab ,
                                                    ufcarttrab ,
                                                    dtcarttrab ,
                                                    nit ,
                                                    cartmotorista,
                                                    tipocarthabilit,
                                                    dtvenchabilit,
                                                    sitmilitar,
                                                    certifreserv,
                                                    categmilitar,
                                                    csm,
                                                    dtexpcml ,
                                                    exped,
                                                    rm,
                                                    npassaporte,
                                                    paisorigem,
                                                    dtemisspassaporte ,
                                                    dtvalpassaporte ,
                                                    corraca ,
                                                    deficientefisico ,
                                                    deficienteauditivo ,
                                                    deficientefala ,
                                                    deficientevisual ,
                                                    deficientemental ,
                                                    recursorealizacaotrab,
                                                    recursoacessibilidade,
                                                    profissao ,
                                                    empresa,
                                                    ocupacao,
                                                    tiposang,
                                                    aluno,
                                                    professor,
                                                    usuariobiblios,
                                                    funcionario,
                                                    exfuncionario,
                                                    candidato FROM SP_TOTVS_PPESSOA";

        private string _queryCountPessoaFiltro = "select count(*) from SP_TOTVS_PPESSOA";

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

            List<PessoaFiltro> pessoa = new List<PessoaFiltro>();            

            error = buscarPessoaFiltro(pessoa);

            _bgWorker.ReportProgress(0, string.Format("Pessoa Gerada..."));

            FileHelperEngine engine = new FileHelperEngine(typeof(PessoaFiltro), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, pessoa);
        }

        private bool buscarPessoaFiltro(List<PessoaFiltro> pessoaFiltroNovo)
        {
            bool error = false;

            _bgWorker.ReportProgress(0, string.Format("Gerando Pessoas no SICA..."));

            string connStr = ConfigurationManager.ConnectionStrings["SICA"].ConnectionString;           

            using (DbConnection connection = new FbConnection(connStr))
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = _queryPessoaFiltro;
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();

                    IDataReader drPessoaFiltro = command.ExecuteReader();

                    double totalRecords = contarPessoas();

                    double processedRecords = 0;

                    while (drPessoaFiltro.Read())
                    {
                        processedRecords++;

                        PessoaFiltro pessoafiltro = new PessoaFiltro();

                        try
                        {
                            pessoafiltro.CPF = drPessoaFiltro["cpf"].ToString();
                            pessoafiltro.Codigo = drPessoaFiltro["codigo"].ToString();
                            pessoafiltro.Nome = drPessoaFiltro["nome"].ToString();
                            pessoafiltro.Apelido = drPessoaFiltro["apelido"].ToString();
                            pessoafiltro.DtNascimento = drPessoaFiltro["dtnascimento"].ToString();
                            pessoafiltro.EstadoCivil = drPessoaFiltro["estadocivil"].ToString();
                            pessoafiltro.Sexo = drPessoaFiltro["sexo"].ToString();
                            pessoafiltro.Naturalidade = drPessoaFiltro["naturalidade"].ToString();
                            pessoafiltro.GrauInstrucao = drPessoaFiltro["grauinstrucao"].ToString();
                            pessoafiltro.Rua = drPessoaFiltro["rua"].ToString();
                            pessoafiltro.Numero = drPessoaFiltro["numero"].ToString();
                            pessoafiltro.Complemento = drPessoaFiltro["complemento"].ToString();
                            pessoafiltro.Bairro = drPessoaFiltro["bairro"].ToString();
                            pessoafiltro.Estado = drPessoaFiltro["estado"].ToString();
                            pessoafiltro.Cidade = drPessoaFiltro["cidade"].ToString();
                            pessoafiltro.CEP = drPessoaFiltro["cep"].ToString();
                            pessoafiltro.Pais = drPessoaFiltro["pais"].ToString();

                            pessoafiltro.RegProfissional = drPessoaFiltro["regprofissional"].ToString();
                            pessoafiltro.CPF = drPessoaFiltro["cpf"].ToString();
                            pessoafiltro.Telefone1 = drPessoaFiltro["telefone1"].ToString();
                            pessoafiltro.Telefone2 = drPessoaFiltro["telefone2"].ToString();
                            pessoafiltro.Telefone3 = drPessoaFiltro["telefone3"].ToString();
                            pessoafiltro.Fax = drPessoaFiltro["fax"].ToString();
                            pessoafiltro.EMail = drPessoaFiltro["email"].ToString();
                            pessoafiltro.CartIdentidade = drPessoaFiltro["cartidentidade"].ToString();

                            pessoafiltro.UFCartIdent = drPessoaFiltro["ufcartident"].ToString();
                            pessoafiltro.OrgEmissorIdent = drPessoaFiltro["orgemissorident"].ToString();
                            pessoafiltro.DtEmissaoIdent = drPessoaFiltro["dtemissaoident"].ToString();
                            pessoafiltro.TituloEleitor = drPessoaFiltro["tituloeleitor"].ToString();
                            pessoafiltro.ZonaTitEleitor = drPessoaFiltro["zonatiteleitor"].ToString();
                            pessoafiltro.SecaoTitEleitor = drPessoaFiltro["secaotiteleitor"].ToString();
                            pessoafiltro.DtTitEleitor = drPessoaFiltro["dttiteleitor"].ToString();

                            pessoafiltro.EstEleit = drPessoaFiltro["esteleit"].ToString();
                            pessoafiltro.CarteiraTrab = drPessoaFiltro["carteiratrab"].ToString();
                            pessoafiltro.SerieCartTrab = drPessoaFiltro["seriecarttrab"].ToString();
                            pessoafiltro.UFCartTrab = drPessoaFiltro["ufcarttrab"].ToString();
                            pessoafiltro.DtCartTrab = drPessoaFiltro["dtcarttrab"].ToString();
                            pessoafiltro.NIT = drPessoaFiltro["nit"].ToString();
                            pessoafiltro.CartMotorista = drPessoaFiltro["cartmotorista"].ToString();
                            pessoafiltro.TipoCartHabilit = drPessoaFiltro["tipocarthabilit"].ToString();

                            pessoafiltro.DtVencHabilit = drPessoaFiltro["dtvenchabilit"].ToString();
                            pessoafiltro.SitMilitar = drPessoaFiltro["sitmilitar"].ToString();
                            pessoafiltro.CertifReserv = drPessoaFiltro["certifreserv"].ToString();
                            pessoafiltro.CategMilitar = drPessoaFiltro["categmilitar"].ToString();
                            pessoafiltro.CSM = drPessoaFiltro["csm"].ToString();
                            pessoafiltro.DtExpCml = drPessoaFiltro["dtexpcml"].ToString();
                            pessoafiltro.Exped = drPessoaFiltro["exped"].ToString();
                            pessoafiltro.RM = drPessoaFiltro["rm"].ToString();

                            pessoafiltro.NPassaporte = drPessoaFiltro["npassaporte"].ToString();
                            pessoafiltro.PaisOrigem = drPessoaFiltro["paisorigem"].ToString();
                            pessoafiltro.DtEmissPassaporte = drPessoaFiltro["dtemisspassaporte"].ToString();
                            pessoafiltro.DtValPassaporte = drPessoaFiltro["dtvalpassaporte"].ToString();
                            pessoafiltro.CorRaca = drPessoaFiltro["corraca"].ToString();
                            pessoafiltro.DeficienteFisico = drPessoaFiltro["deficientefisico"].ToString();
                            pessoafiltro.DeficienteAuditivo = drPessoaFiltro["deficienteauditivo"].ToString();
                            pessoafiltro.DeficienteFala = drPessoaFiltro["deficientefala"].ToString();
                            pessoafiltro.DeficienteVisual = drPessoaFiltro["deficientevisual"].ToString();

                            pessoafiltro.DeficienteMental = drPessoaFiltro["deficientemental"].ToString();
                            pessoafiltro.RecursoRealizacaoTrab = drPessoaFiltro["recursorealizacaotrab"].ToString();
                            pessoafiltro.RecursoAcessibilidade = drPessoaFiltro["recursoacessibilidade"].ToString();
                            pessoafiltro.Profissao = drPessoaFiltro["profissao"].ToString();
                            pessoafiltro.Empresa = drPessoaFiltro["empresa"].ToString();
                            pessoafiltro.Ocupacao = drPessoaFiltro["ocupacao"].ToString();
                            pessoafiltro.TipoSang = drPessoaFiltro["tiposang"].ToString();

                            pessoafiltro.Aluno = drPessoaFiltro["aluno"].ToString();
                            pessoafiltro.Professor = drPessoaFiltro["professor"].ToString();
                            pessoafiltro.UsuarioBiblios = drPessoaFiltro["usuariobiblios"].ToString();
                            pessoafiltro.Funcionario = drPessoaFiltro["funcionario"].ToString();
                            pessoafiltro.ExFuncionario = drPessoaFiltro["exfuncionario"].ToString();
                            pessoafiltro.Candidato = drPessoaFiltro["candidato"].ToString();

                            
                            //pessoaFiltroNovo.Add(pessoafiltro);

                            if (!PessoaDAO.existePessoa(pessoafiltro.CPF) && pessoafiltro.CPF != null)
                                pessoaFiltroNovo.Add(pessoafiltro);
                            else
                                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), string.Format("Não foi possível exportar a PESSOA NOME: {0}, CPF: {1}., motivo: CPF já cadastrado", pessoafiltro.Nome, pessoafiltro.CPF));

                            

                        }
                        catch (Exception ex)
                        {
                            error = true;
                            _bgWorker.ReportProgress(0, string.Format("Erro: {0}", ex.Message));
                        }
                        
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }

                    drPessoaFiltro.Close();
                }
                catch (Exception ex)
                {
                    error = true;
                    _bgWorker.ReportProgress(0, string.Format("Erro: {0}", ex.Message));
                }
                return error;
            }            

        }

        private double contarPessoas()
        {
            _bgWorker.ReportProgress(0, string.Format("Contando Pessoas no SICA..."));

            string connStr = ConfigurationManager.ConnectionStrings["SICA"].ConnectionString;

            using (DbConnection connection = new FbConnection(connStr))
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = _queryCountPessoaFiltro;
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();

                    double count = Convert.ToDouble(command.ExecuteScalar());

                    return count;
                }
                catch (Exception ex)
                {
                    error = true;
                    _bgWorker.ReportProgress(0, string.Format("Erro: {0}", ex.Message));
                }
            }

            return 0;
        }
    
    }

    
}
