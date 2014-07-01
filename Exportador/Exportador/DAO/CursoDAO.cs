using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Exportador.Academico.Curso;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.Helpers;

namespace Exportador.DAO
{
    public class CursoDAO
    {
        private string _queryCursos = @"SELECT
                                            C.ID as CODCURSO,
                                            IIF(C.NOME_OFICIAL IS NULL,C.NOME,C.NOME_OFICIAL) as NOME,
                                            C.NOME as COMPLEMENTO,
                                            TRIM(C.LEI_AUTORIZACAO) as DECRETO,
                                            C.HABILITACAO as HABILITACAO,
                                            C.ID_AREA_DO_CONHECIMENTO as IDAREA,
                                            NC.NOME AS NIVELCURSO
                                        FROM CURSO C
                                        INNER JOIN NIVEL_CURSO NC ON NC.ID=C.ID_NIVEL_CURSO
                                        WHERE C.ID=@ID_CURSO";

        private string _queryCursosDestino = @"SELECT [CODCOLIGADA]
                                                  ,[CODTIPOCURSO]
                                                  ,[CODCURSO]
                                                  ,[NOME]      
                                                  ,[DESCRICAO]
                                                  ,[COMPLEMENTO]
                                                  ,[CODCURINEP]
                                                  ,[DECRETO]
                                                  ,[REGCONTRATO]
                                                  ,[CFGMATRICULA]
                                                  ,[HABILITACAO]
                                                  ,[CAPES]               
                                                  ,[CURPRESDIST]
                                                  ,[CODMODALIDADECURSO]
                                              FROM [SCURSO]";

        public Curso BuscaCursoPorID(int idCurso)
        {
            Curso curso = new Curso();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            using (DbCommand command = database.GetSqlStringCommand(_queryCursos))
            {
                database.AddInParameter(command, "@ID_CURSO", DbType.String, idCurso);

                using (IDataReader drCurso = database.ExecuteReader(command))
                {
                    while (drCurso.Read())
                    {
                        curso.CodTipoCurso = buscarTipoCurso(drCurso["NIVELCURSO"].ToString(),drCurso["NOME"].ToString());
                        curso.CodCurso = drCurso["CODCURSO"].ToString();
                        curso.Nome = drCurso["NOME"].ToString().RemoveSpecialChars();
                        curso.Complemento = drCurso["Complemento"].ToString().RemoveSpecialChars();
                        curso.Decreto = drCurso["DECRETO"].ToString().RemoveSpecialChars();
                        curso.Area = buscarAreaConhecimento(drCurso["IDAREA"].ToString()).RemoveSpecialChars();

                        curso.CodColigada = 1;
                        curso.Habilitacao = curso.Nome;
                    }
                }
            }

            return curso;
        }

        private string buscarAreaConhecimento(string idArea)
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

            if (String.IsNullOrEmpty(area))
                throw new BusinessException("Curso não possui área de conhecimento relacionada.");

            return area;
        }

        /// <summary>
        /// Busca o tipo de curso (contexto) do sistema destino, com base no tipo de curso de origem.
        /// </summary>
        /// <param name="tipoCurso">Tipo de custo origem.</param>
        /// <returns></returns>
        public int buscarTipoCurso(string tipoCurso)
        {
            int _tipo = 0;

            switch (tipoCurso)
            {
                case "ESPECIALIZACAO":
                case "EXTENSAO":
                case "MESTRADO":
                case "APERFEICOAMENTO":

                    _tipo = TipoCurso.PosGraduacaoMestrado;
                    break;
                case "GRADUACAO":
                    _tipo = TipoCurso.EnsinoSuperiorGraduacaoLagesSC;
                    break;
                default:
                    _tipo = 0;
                    break;
            }

            if (_tipo == 0)
                throw new BusinessException(String.Format("Tipo de curso '{0}' não possui relacionamento.",tipoCurso));

            return _tipo;
        }

        /// <summary>
        /// Busca o tipo de curso (contexto) do sistema destino, com base no tipo de curso de origem.
        /// </summary>
        /// <param name="nivelCurso">Nível de graduação do curso. (Graduação, Poé-Graduação, Mestrado...)</param>
        /// <param name="nomeCurso">Nome do curso.</param>
        /// <returns></returns>
        public int buscarTipoCurso(string tipoCurso,string nomeCurso)
        {
            nomeCurso = nomeCurso.RemoveSpecialChars().ToUpper();

            if ((nomeCurso.Contains("SAO JOAQUIM")) && (nomeCurso.Contains("GRADUACAO")))
                return TipoCurso.EnsinoSuperiorGraduacaoSaoJoaquimSC;

            return buscarTipoCurso(tipoCurso.RemoveSpecialChars().ToUpper());
        }

        /// <summary>
        /// Retorna todos os cursos importados para o sistema de destino.
        /// </summary>
        /// <returns></returns>
        public List<Curso> buscarTodosDestino()
        {
            return DBHelper.GetAll("RM", _queryCursosDestino, String.Empty, CursoConverter);
        }

        private Curso CursoConverter(IDataReader reader)
        {
            Curso c = new Curso();

            c.CodColigada = (int)reader.GetNullableInt32("CODCOLIGADA");
            c.CodTipoCurso = (int)reader.GetNullableInt32("CODTIPOCURSO");
            c.CodCurso = reader.GetString("CODCURSO");
            c.Nome = reader.GetString("NOME");
            c.Descricao = reader.GetString("DESCRICAO");
            c.Complemento = reader.GetString("COMPLEMENTO");
            c.CodCursoINEP = reader.GetString("CODCURINEP");
            c.Decreto = reader.GetString("DECRETO");
            c.RegContrato = reader.GetString("REGCONTRATO");
            c.MascaraRA = reader.GetString("CFGMATRICULA");
            c.Habilitacao = reader.GetString("HABILITACAO");
            c.CodCapes = reader.GetString("CAPES");
            c.CursoPresDist = reader.GetString("CURPRESDIST");
            c.CodModalidadeCurso = reader.GetString("CODMODALIDADECURSO");

            return c;
        }
    }
}
