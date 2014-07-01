using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Exportador.Helpers;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.Academico.Habilitacao
{
    public class ExportadorSHabilitacao : IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool _debugMode;

        #endregion

        #region Queries

        private string _queryHabilitacoes = @"SELECT DISTINCT
                                                C.ID AS CODCURSO,
                                                C.HABILITACAO AS HABILITACAO,
                                                IIF(C.NOME_OFICIAL IS NULL,C.NOME,C.NOME_OFICIAL) as NOMECURSO,
                                                C.LEI_AUTORIZACAO AS DECRETO
                                            FROM
                                            CURSO C
                                            INNER JOIN MATRICULA_CURSO MC ON MC.ID_CURSO=C.ID
                                            WHERE C.ID NOT IN (0,10);
        ";

        #endregion

        private string _queryFuncoes;

        #region Properties

        /// <summary>
        /// Número de registros totais a ser retornados. 
        /// 0 para todos.
        /// </summary>
        public int RecordsCount
        {
            get { return _recordsToReturn; }
            set { _recordsToReturn = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
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
        public ExportadorSHabilitacao()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorSHabilitacao(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorSHabilitacao(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion


        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            List<Habilitacao> habilitacoes = new List<Habilitacao>();

            habilitacoes.AddRange(buscarHabilitacoes());

            excluirCursosNaoCadastrados(habilitacoes);

            FileHelperEngine engine = new FileHelperEngine(typeof(Habilitacao), Encoding.Unicode);

            engine.WriteFile(_filename, habilitacoes);
        }

        private List<Habilitacao> buscarHabilitacoes()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand command = database.GetSqlStringCommand(_queryHabilitacoes);

            IDataReader drHabilitacoes = database.ExecuteReader(command);

            List<Habilitacao> lHabilitacoes = new List<Habilitacao>();

            //int codHabilitacao = 1;

            while (drHabilitacoes.Read())
            {
                Habilitacao habilitacao = new Habilitacao();

                habilitacao = mapear(drHabilitacoes);

                lHabilitacoes.Add(habilitacao);



                //codHabilitacao++;
            }

            return lHabilitacoes;
        }

        private Habilitacao mapear(IDataRecord record)
        {
            Habilitacao hab = new Habilitacao();

            hab.CodCurso = record["CodCurso"].ToString();
            hab.CodHabilitacao = record["CodCurso"].ToString();
            hab.Complemento = record["NOMECURSO"].ToString().RemoveSpecialChars();
            hab.Decreto = record["Decreto"].ToString().RemoveSpecialChars();
            hab.Nome = buscarNome(record).Replace(";","-");

            hab.CodColigada = 1;

            return hab;
        }

        private string buscarNome(IDataRecord record)
        {
            switch (record["Habilitacao"].ToString())
            {
                case "":
                case ",":
                case "-":
                case ".":
                case "..":
                case "...":
                case "a":
                case "aaa":
                    return record["NomeCurso"].ToString();

                default:
                    return record["Habilitacao"].ToString();
            }
        }

        private void excluirCursosNaoCadastrados(List<Habilitacao> habilitacoes)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            List<string> cursosRM = new List<string>();

            using (DbCommand command = database.GetSqlStringCommand("SELECT DISTINCT CODCURSO FROM SCURSO"))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    cursosRM.Add(reader.GetString("CODCURSO"));
                }
            }

            List<Habilitacao> habFiltrada = new List<Habilitacao>();

            foreach (var codCurso in cursosRM)
            {
                habFiltrada.AddRange(habilitacoes.Where(h => h.CodCurso == codCurso));
            }

            habilitacoes.Clear();

            habilitacoes.AddRange(habFiltrada);
        }
    }
}
