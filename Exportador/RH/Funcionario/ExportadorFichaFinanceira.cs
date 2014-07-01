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
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Configuration;

namespace Exportador.RH.Funcionario
{
    public class ExportadorFichaFinanceira: IExportador
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
        public ExportadorFichaFinanceira()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorFichaFinanceira(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorFichaFinanceira(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryFichas = @"select * from (
select 
	chapa.Chapa as Chapa
	,DATEPART(YEAR,calculo.perref) as AnoCompetencia
	,DATEPART(MONTH,calculo.perref) as MesCompetencia
	, CASE 
			when MONTH(calculo.datpag) = '11' and MONTH(calculo.perref) = '11' then '30'
			when MONTH(calculo.datpag) = '12' and MONTH(calculo.perref) = '12' then '35'
			else '20'
		end as NPeriodo
			,case 
		when fichaFinanc.codeve = '58' or evento.deseve = '1/3 de Férias' then '0040'
        when fichaFinanc.codeve = '1359' or evento.deseve = '1/3 de Férias' then '0040'
        when fichaFinanc.codeve = '1379' or evento.deseve = '1/3 de Férias Resc.' then '0063'
        when fichaFinanc.codeve = '67' or evento.deseve = '1/3 Férias Rescisäo' then '0063'
        when fichaFinanc.codeve = '65' or evento.deseve = '13.º Indenizado Rescisão' then '0071'
        when fichaFinanc.codeve = '50' or evento.deseve = '13º Salário Adiantado' then '0009'
        when fichaFinanc.codeve = '46' or evento.deseve = '13º Salário Complementar' then '0066'
        when fichaFinanc.codeve = '51' or evento.deseve = '13º Salário Integral' then '0102'
        when fichaFinanc.codeve = '97' or evento.deseve = '13º Salário Prop.Rescisäo' then '0048'
        when fichaFinanc.codeve = '57' or evento.deseve = 'Abono Pecuniário de férias' then '0039'
        when fichaFinanc.codeve = '1827' or evento.deseve = 'ACORDO JUDICIAL/EXTA JUD' then '1035'
        when fichaFinanc.codeve = '270' or evento.deseve = 'Adic.Noturno 13º Inden.' then '9007'
        when fichaFinanc.codeve = '110' or evento.deseve = 'Adic.Noturno A.P.I' then '1028'
        when fichaFinanc.codeve = '273' or evento.deseve = 'Adic.Noturno API' then '1028'
        when fichaFinanc.codeve = '27' or evento.deseve = 'Adicional Noturno' then '0405'
        when fichaFinanc.codeve = '1382' or evento.deseve = 'ADICIONAL NOTURNO API' then '1028'
        when fichaFinanc.codeve = '1330' or evento.deseve = 'ADICIONAL NOTURNO PROF.' then '1025'
        when fichaFinanc.codeve = '428' or evento.deseve = 'ADUNIPLAC MENSALIDADE' then '1038'
        when fichaFinanc.codeve = '1826' or evento.deseve = 'AJUDA EXTRA_CLASSE' then '1034'
        when fichaFinanc.codeve = '414' or evento.deseve = 'AULAS ENS SUP C/DSR(V)' then '1005'
        when fichaFinanc.codeve = '527' or evento.deseve = 'Auxilio Funeral' then '1058'
        when fichaFinanc.codeve = '69' or evento.deseve = 'Aviso Prévio Indenizado' then '0062'
        when fichaFinanc.codeve = '1380' or evento.deseve = 'Aviso Prévio Indenizado' then '0062'
        when fichaFinanc.codeve = '113' or evento.deseve = 'Aviso Prévio Trab.Noturno' then '0062'
        when fichaFinanc.codeve = '1828' or evento.deseve = 'BOLSA DE PESQUISA' then '1036'
        when fichaFinanc.codeve = '1830' or evento.deseve = 'Cerimoniais' then '1065'
        when fichaFinanc.codeve = '1028' or evento.deseve = 'CLINICA INTEGRADA ODONTO' then '1008'
        when fichaFinanc.codeve = '1829' or evento.deseve = 'Comissão comercial' then '1060'
        when fichaFinanc.codeve = '610' or evento.deseve = 'COMPLEMENTO DE SALÁRIO' then '1006'
        when fichaFinanc.codeve = '141' or evento.deseve = 'CONT.CONF.CLAUS.CCT/SAAER' then '1069'
        when fichaFinanc.codeve = '148' or evento.deseve = 'Contrib. Sindical Horista' then '0012'
        when fichaFinanc.codeve = '854' or evento.deseve = 'CONTRIBUIÇÃO CONFEDERATIV' then '1067'
        when fichaFinanc.codeve = '147' or evento.deseve = 'Contribuiçäo Sindical' then '0012'
        when fichaFinanc.codeve = '959' or evento.deseve = 'desc. 13 sala. adiant' then '0035'
        when fichaFinanc.codeve = '156' or evento.deseve = 'DESC. ADIANT.SALARIAL' then '0019'
        when fichaFinanc.codeve = '1820' or evento.deseve = 'DESC. ADUNIPLAC DIVERSOS' then '1043'
        when fichaFinanc.codeve = '948' or evento.deseve = 'DESC. DIFERENCA SALARIAL' then '1079'
        when fichaFinanc.codeve = '1221' or evento.deseve = 'DESC.BANCO BANRISUL (Consignado)' then '0036'
        when fichaFinanc.codeve = '846' or evento.deseve = 'DESC.SALARIO PAGO MAIOR' then '1061'
        when fichaFinanc.codeve = '1430' or evento.deseve = 'Desconto 13ºSal.Adiantado (Professor)' then '0035'
        when fichaFinanc.codeve = '138' or evento.deseve = 'Desconto Adto Férias' then '0043'
        when fichaFinanc.codeve = '427' or evento.deseve = 'DESCONTO AFEUP' then '1037'
        when fichaFinanc.codeve = '839' or evento.deseve = 'DESCONTO AUTORIZADO' then '1039'
        when fichaFinanc.codeve = '1226' or evento.deseve = 'DESCONTO BANCO BRASIL' then '1053'
        when fichaFinanc.codeve = '1085' or evento.deseve = 'DESCONTO SOCIO SAAERS' then '1041'
        when fichaFinanc.codeve = '1133' or evento.deseve = 'DESCONTO SOCIO SINPROESC' then '1042'
        when fichaFinanc.codeve = '1061' or evento.deseve = 'DESCONTO TELEFONE' then '1040'
        when fichaFinanc.codeve = '686' or evento.deseve = 'Dev. Desconto Indevido' then '1063'
        when fichaFinanc.codeve = '1825' or evento.deseve = 'Dev.Desc.a maior Férias' then '1080'
        when fichaFinanc.codeve = '1824' or evento.deseve = 'Dif. Triênio mês anterior' then '1064'
        when fichaFinanc.codeve = '35' or evento.deseve = 'Dif.Salario Mes Anterior' then '0037'
        when fichaFinanc.codeve = '113' or evento.deseve = 'Dif.triênio 3% pg.a menor' then '1057'
        when fichaFinanc.codeve = '60' or evento.deseve = 'Diferença de Férias' then '0045'
        when fichaFinanc.codeve = '973' or evento.deseve = 'Diferenca de Maternidade' then '1086'
        when fichaFinanc.codeve = '270' or evento.deseve = 'Diferença de Salário' then '0037'
        when fichaFinanc.codeve = '139' or evento.deseve = 'Diferença IRRF Férias' then '1056'
        when fichaFinanc.codeve = '42' or evento.deseve = 'DSR (VALOR)' then '1003'
        when fichaFinanc.codeve = '1813' or evento.deseve = 'DSR HORAS PROFICIËNCIA' then '1031'
        when fichaFinanc.codeve = '45' or evento.deseve = 'DSR s/ Horas Extras' then '0404'
        when fichaFinanc.codeve = '141' or evento.deseve = 'Estabilidade Rescisäo' then '1072'
        when fichaFinanc.codeve = '191' or evento.deseve = 'Estouro do Mês' then '0020'
        when fichaFinanc.codeve = '190' or evento.deseve = 'Estouro Mês Anterior' then '0021'
        when fichaFinanc.codeve = '78' or evento.deseve = 'Férias Proporc.Rescisäo' then '0025'
        when fichaFinanc.codeve = '178' or evento.deseve = 'Ferias S/Rescisao' then '0025'
        when fichaFinanc.codeve = '66' or evento.deseve = 'Férias Vencidas Rescisäo' then '0024'
        when fichaFinanc.codeve = '110' or evento.deseve = 'FGTS' then '1089'
        when fichaFinanc.codeve = '117' or evento.deseve = 'FGTS 13. SAL. RESCISAO' then '0031'
        when fichaFinanc.codeve = '136' or evento.deseve = 'FGTS 13. SALARIO' then '0031'
        when fichaFinanc.codeve = '73' or evento.deseve = 'FGTS 40% Rescisäo (Valor)' then '0028'
        when fichaFinanc.codeve = '72' or evento.deseve = 'FGTS Rescisäo (Valor)' then '0026'
        when fichaFinanc.codeve = '1453' or evento.deseve = 'Gratif. Aux. Maternidade' then '1047'
        when fichaFinanc.codeve = '875' or evento.deseve = 'GRATIFICAÇÃO DE FUNÇÃO' then '1007'
        when fichaFinanc.codeve = '1823' or evento.deseve = 'H.PROC.IMPL.OP.BCO DENTES' then '1033'
        when fichaFinanc.codeve = '1452' or evento.deseve = 'HORAS AF.AUX.MATERNIDADE' then '1029'
        when fichaFinanc.codeve = '24' or evento.deseve = 'Horas Afast.Acid.Trabalho' then '1084'
        when fichaFinanc.codeve = '1451' or evento.deseve = 'Horas Afast.Aux.Doenca' then '1088'
        when fichaFinanc.codeve = '25' or evento.deseve = 'Horas Afast.Aux.Doença' then '1088'
        when fichaFinanc.codeve = '1313' or evento.deseve = 'HORAS ASSESSORES' then '1019'
        when fichaFinanc.codeve = '16' or evento.deseve = 'HORAS AUX.PATERNIDADE' then '1002'
        when fichaFinanc.codeve = '12' or evento.deseve = 'HORAS AUXÍLIO MATERNIDADE' then '0017'
        when fichaFinanc.codeve = '1821' or evento.deseve = 'HORAS COMISSÕES ANÁLISE' then '1032'
        when fichaFinanc.codeve = '1500' or evento.deseve = 'Horas Controle de Jornada' then '1082'
        when fichaFinanc.codeve = '1307' or evento.deseve = 'HORAS COORDENAÇÃO CURSO' then '1014'
        when fichaFinanc.codeve = '1320' or evento.deseve = 'HORAS DSR' then '1022'
        when fichaFinanc.codeve = '1321' or evento.deseve = 'HORAS DSR COORD/MEST/CHEF' then '1023'
        when fichaFinanc.codeve = '1358' or evento.deseve = 'HORAS DSR FÉRIAS' then '1027'
        when fichaFinanc.codeve = '1818' or evento.deseve = 'Horas Ens Sup FUMDES BR' then '1062'
        when fichaFinanc.codeve = '1822' or evento.deseve = 'Horas Ens Sup FUMDES Lg' then '1068'
        when fichaFinanc.codeve = '1816' or evento.deseve = 'Horas Ens Sup FUMDES SJ' then '1071'
        when fichaFinanc.codeve = '1819' or evento.deseve = 'Horas Ens Sup Intensivo' then '1081'
        when fichaFinanc.codeve = '1300' or evento.deseve = 'HORAS ENSINO SUPERIOR' then '1009'
        when fichaFinanc.codeve = '1314' or evento.deseve = 'HORAS ENSINO SUPERIOR' then '1020'
        when fichaFinanc.codeve = '21' or evento.deseve = 'Horas Extras c/ 50%' then '0406'
        when fichaFinanc.codeve = '23' or evento.deseve = 'Horas Extras Noturna 50%' then '1078'
        when fichaFinanc.codeve = '1510' or evento.deseve = 'Horas Falta' then '0008'
        when fichaFinanc.codeve = '23' or evento.deseve = 'Horas Faltas DSR Noturno' then '1087'
        when fichaFinanc.codeve = '7' or evento.deseve = 'Horas Ferias Diurnas' then '0041'
        when fichaFinanc.codeve = '8' or evento.deseve = 'Horas Férias Noturnas' then '0041'
        when fichaFinanc.codeve = '1312' or evento.deseve = 'HORAS GESTOR' then '1018'
        when fichaFinanc.codeve = '1306' or evento.deseve = 'Horas Mestre' then '1070'
        when fichaFinanc.codeve = '1' or evento.deseve = 'Horas Normais' then '0001'
        when fichaFinanc.codeve = '2' or evento.deseve = 'Horas Normais Noturnas' then '0001'
        when fichaFinanc.codeve = '1309' or evento.deseve = 'HORAS ORIENT.EST/TCC/MON' then '1016'
        when fichaFinanc.codeve = '1812' or evento.deseve = 'HORAS PROFICIÊNCIAS' then '1030'
        when fichaFinanc.codeve = '1302' or evento.deseve = 'HORAS PROJETO EXTENSÃO' then '1011'
        when fichaFinanc.codeve = '1303' or evento.deseve = 'HORAS PROJETO PESQUISA' then '1012'
        when fichaFinanc.codeve = '1305' or evento.deseve = 'HORAS PROJETO PG MESTRADO' then '1013'
        when fichaFinanc.codeve = '3' or evento.deseve = 'HORAS REPOUSO REM.DIURNO' then '1001'
        when fichaFinanc.codeve = '1317' or evento.deseve = 'Horas Reunioes/Conferenc.' then '1055'
        when fichaFinanc.codeve = '1311' or evento.deseve = 'HORAS SECRETÁRIOS' then '1017'
        when fichaFinanc.codeve = '25' or evento.deseve = 'Horas Ser.Militar Noturna' then '1083'
        when fichaFinanc.codeve = '24' or evento.deseve = 'Horas Serviço Militar' then '1085'
        when fichaFinanc.codeve = '1308' or evento.deseve = 'HORAS SUPERV. EST/TCC/MON' then '1015'
        when fichaFinanc.codeve = '1832' or evento.deseve = 'Horas tradução linguas' then '1075'
        when fichaFinanc.codeve = '1318' or evento.deseve = 'Horas Turmas Especiais' then '1073'
        when fichaFinanc.codeve = '1806' or evento.deseve = 'Hs Ates.Med. ate 15dias N' then '0145'
        when fichaFinanc.codeve = '14' or evento.deseve = 'Hs Ates.Medico ate 15dias' then '0145'
        when fichaFinanc.codeve = '1319' or evento.deseve = 'HS BANCAS EST/TCC/MON/PRO' then '1021'
        when fichaFinanc.codeve = '1506' or evento.deseve = 'Ind. Térm. Contr. Antec.' then '0124'
        when fichaFinanc.codeve = '102' or evento.deseve = 'IND.ART.9º LEI 6708' then '1000'
        when fichaFinanc.codeve = '1805' or evento.deseve = 'Indenização' then '1076'
        when fichaFinanc.codeve = '72' or evento.deseve = 'Insalubridade 13º Integ.' then '1092'
        when fichaFinanc.codeve = '66' or evento.deseve = 'Insalubridade s/ Férias' then '1093'
        when fichaFinanc.codeve = '120' or evento.deseve = 'INSS' then '0003'
        when fichaFinanc.codeve = '122' or evento.deseve = 'INSS s/ 13º Salário' then '0011'
        when fichaFinanc.codeve = '124' or evento.deseve = 'INSS s/ Férias' then '0082'
        when fichaFinanc.codeve = '130' or evento.deseve = 'IRRF' then '0004'
        when fichaFinanc.codeve = '132' or evento.deseve = 'IRRF s/ 13º Salário' then '0049'
        when fichaFinanc.codeve = '134' or evento.deseve = 'IRRF s/ Férias' then '0030'
        when fichaFinanc.codeve = '32' or evento.deseve = 'LICENÇA REMUNERADA' then '0402'
        when fichaFinanc.codeve = '105' or evento.deseve = 'Líquido Rescisäo' then '0399'
        when fichaFinanc.codeve = '202' or evento.deseve = 'Média H.Extra 13º Inden.' then '9017'
        when fichaFinanc.codeve = '199' or evento.deseve = 'Média H.Extra 13º Prop.Re' then '9017'
        when fichaFinanc.codeve = '205' or evento.deseve = 'Média H.Extra Férias Resc' then '9012'
        when fichaFinanc.codeve = '202' or evento.deseve = 'Média H.Extras 13º Adto' then '9015'
        when fichaFinanc.codeve = '117' or evento.deseve = 'Média H.Extras Grat.Resc.' then '1091'
        when fichaFinanc.codeve = '86' or evento.deseve = 'Média Hs.Extra API' then '1090'
        when fichaFinanc.codeve = '203' or evento.deseve = 'Média Variáv. 13º Inden.' then '9048'
        when fichaFinanc.codeve = '1401' or evento.deseve = 'Média Variáv.13ºInd.Resc' then '9016'
        when fichaFinanc.codeve = '87' or evento.deseve = 'Média Variáveis API' then '1095'
        when fichaFinanc.codeve = '1381' or evento.deseve = 'Média Variáveis API' then '1095'
        when fichaFinanc.codeve = '136' or evento.deseve = 'Média Variáveis Férias' then '9041'
        when fichaFinanc.codeve = '119' or evento.deseve = 'Pensäo Judicial' then '0013'
        when fichaFinanc.codeve = '186' or evento.deseve = 'Pensäo Judicial 13º Sal.' then '0120'
        when fichaFinanc.codeve = '396' or evento.deseve = 'Pensäo Judicial s/Ferias' then '0113'
        when fichaFinanc.codeve = '1999' or evento.deseve = 'Período Sem Atividade' then '1077'
        when fichaFinanc.codeve = '1062' or evento.deseve = 'PIS na Folha (abono/Rend)' then '1054'
        when fichaFinanc.codeve = '1301' or evento.deseve = 'PÓS-GRADUAÇÃO' then '1010'
        when fichaFinanc.codeve = '1831' or evento.deseve = 'Projeto de Extensão ' then '1074'
        when fichaFinanc.codeve = '325' or evento.deseve = 'REP.VESTIBULAR/CONCURSOS' then '1004'
        when fichaFinanc.codeve = '1336' or evento.deseve = 'RES.MEDICA MIN.SAÚDE' then '1026'
        when fichaFinanc.codeve = '1337' or evento.deseve = 'Res.Multiprofissional' then '1066'
        when fichaFinanc.codeve = '1324' or evento.deseve = 'SALÁRIO ADM' then '1024'
        when fichaFinanc.codeve = '31' or evento.deseve = 'Salario Familia' then '0005'
        when fichaFinanc.codeve = '101' or evento.deseve = 'Saldo de Salário' then '0401'
        when fichaFinanc.codeve = '131' or evento.deseve = 'SUPERV.ESTAGIO PSICOLOGIA' then '1059'
        when fichaFinanc.codeve = '1550' or evento.deseve = 'Triênio' then '0403'
        when fichaFinanc.codeve = '205' or evento.deseve = 'Triênio 13º Adto' then '1049'
        when fichaFinanc.codeve = '1409' or evento.deseve = 'Triênio 13o Sal.Ind.Resc' then '1052'
        when fichaFinanc.codeve = '1399' or evento.deseve = 'Triênio 13o Sal.Prop.Resc' then '1052'
        when fichaFinanc.codeve = '1389' or evento.deseve = 'Triênio A.P.I' then '1094'
        when fichaFinanc.codeve = '1551' or evento.deseve = 'Triênio s/ Férias ' then '1048'
        when fichaFinanc.codeve = '1377' or evento.deseve = 'Triênio s/ Férias Resc.' then '1048'
        when fichaFinanc.codeve = '152' or evento.deseve = 'Vale Transporte' then '0006'
        when fichaFinanc.codeve = '1400' or evento.deseve = '13º Sal.Ind.Resc.Méd.Hor' then '9068'
        when fichaFinanc.codeve = '1390' or evento.deseve = '13º Sal.Prop.Resc.Méd.Hor' then '9068'
        when fichaFinanc.codeve = '1410' or evento.deseve = '13ºSal.Adiant.Méd.Horas' then '9066'
        when fichaFinanc.codeve = '1425' or evento.deseve = '13ºSal.Int.Matern.Med.Hor' then '9071'
        when fichaFinanc.codeve = '1420' or evento.deseve = '13ºSal.Integal Méd.Horas' then '9067'
        when fichaFinanc.codeve = '1392' or evento.deseve = 'Ad.Not.13º Sal.Prop.Resc.' then '9007'
        when fichaFinanc.codeve = '1402' or evento.deseve = 'Ad.Noturno 13ºInd.Resc' then '9007'
        when fichaFinanc.codeve = '1373' or evento.deseve = 'Adic.Noturno Férias Resc.' then '9002'
        when fichaFinanc.codeve = '1353' or evento.deseve = 'Adic.Noturno s/Férias' then '9001'
        when fichaFinanc.codeve = '1408' or evento.deseve = 'Horas DSR 13ºInd.Resc.' then '9037'
        when fichaFinanc.codeve = '1398' or evento.deseve = 'Horas DSR 13ºProp.Resc.' then '9037'
        when fichaFinanc.codeve = '1388' or evento.deseve = 'Horas DSR API' then '9082'
        when fichaFinanc.codeve = '1378' or evento.deseve = 'Horas DSR Férias Resc.' then '9032'
        when fichaFinanc.codeve = '1418' or evento.deseve = 'Horas DSR s/13ºSal.Adiant' then '9035'
        when fichaFinanc.codeve = '1428' or evento.deseve = 'Horas DSR s/13ºSal.Integ' then '9036'
        when fichaFinanc.codeve = '1350' or evento.deseve = 'Horas Férias - Médias' then '9062'
        when fichaFinanc.codeve = '1371' or evento.deseve = 'Horas Férias Prop. Médias' then '9064'
        when fichaFinanc.codeve = '1370' or evento.deseve = 'Horas Férias Venc. Médias' then '9063'
        when fichaFinanc.codeve = '1391' or evento.deseve = 'Média Variáv.13ºProp.Resc' then '9078'
        when fichaFinanc.codeve = '1372' or evento.deseve = 'Médias Férias Vlr.Vár' then '9041'
        when fichaFinanc.codeve = '1352' or evento.deseve = 'Médias Férias Vlr.Vár.' then '9072'
        when fichaFinanc.codeve = '1556' or evento.deseve = 'Triênio 13 Sal.Adto.' then '1049'
        when fichaFinanc.codeve = '964' or evento.deseve = '13 Salario Media Valores' then '9079'
        when fichaFinanc.codeve = '106' or evento.deseve = '13º Sal./Aux.Maternidade' then '9081'
        when fichaFinanc.codeve = '1411' or evento.deseve = '13ºSal.Adiant.Méd.Vlr.Var' then '9076'
        when fichaFinanc.codeve = '1421' or evento.deseve = '13ºSal.Integ.Méd.Vlr.Var' then '9077'
        when fichaFinanc.codeve = '261' or evento.deseve = 'Adic.Noturno 13º Adto' then '9005'
        when fichaFinanc.codeve = '63' or evento.deseve = 'Adic.Noturno 13º Salário' then '9006'
        when fichaFinanc.codeve = '1557' or evento.deseve = 'Triênio 13o Sal.Int.' then '1050'
        when fichaFinanc.codeve = '1560' or evento.deseve = 'Trienio Maternidade' then '1051'
        when fichaFinanc.codeve = '1354' or evento.deseve = 'Triênio s/ Férias' then '1048'
        when fichaFinanc.codeve = '266' or evento.deseve = 'Adic.Noturno 13º Proporc.' then '9007'
        when fichaFinanc.codeve = '265' or evento.deseve = 'Adic.Noturno Férias Resc.' then '9002'
        when fichaFinanc.codeve = '262' or evento.deseve = 'Adic.Noturno s/Férias' then '9001'
        when fichaFinanc.codeve = '206' or evento.deseve = 'Férias Media Rescisao' then '9073'
        when fichaFinanc.codeve = '1227' or evento.deseve = 'Gratificação 13º Adiantado' then '1045'
        when fichaFinanc.codeve = '287' or evento.deseve = 'Gratificaçäo 13º Integral' then '1046'
        when fichaFinanc.codeve = '1817' or evento.deseve = 'Gratificação Férias' then '1044'
        when fichaFinanc.codeve = '179' or evento.deseve = 'Med.Hrs.13º Sal.Prop.Resc' then '9068'
        when fichaFinanc.codeve = '258' or evento.deseve = 'Média Hs.Extra 13º Adto' then '9015'
        when fichaFinanc.codeve = '48' or evento.deseve = 'Média Hs.Extra s/13º Sal.' then '9016'
        when fichaFinanc.codeve = '54' or evento.deseve = 'Média Hs.Extra s/Férias' then '9021'
        when fichaFinanc.codeve = '200' or evento.deseve = 'Média Variáv.13º Prop.Res' then '9048'
        when fichaFinanc.codeve = '55' or evento.deseve = 'Média Vlr.Var. s/Férias' then '9041'
	end	as 'CodEvento',
	calculo.datpag as 'DtPagto'
	,'' as Hora
	,fichaFinanc.refeve as Referencia
	,fichaFinanc.valeve as Valor
	,fichaFinanc.valeve as ValorOriginal
from vetorh.r046ver as fichaFinanc
inner join vetorh.r034fun funcionario on funcionario.numemp=fichaFinanc.numemp
    and funcionario.tipcol=fichaFinanc.tipcol
    and funcionario.numcad=fichaFinanc.numcad
inner join dbo.vw_totvs_chapafuncionario chapa on fichaFinanc.numcad = chapa.numcad
												and fichaFinanc.tipcol = chapa.tipcol    
inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
inner join vetorh.r044cal calculo on calculo.numemp=fichaFinanc.numemp
	and calculo.codcal=fichaFinanc.codcal
inner join vetorh.r008evc evento on fichaFinanc.codeve = evento.codeve 
where chapa.numcpf <> '0'    
) as fichas
where AnoCompetencia >= 2012 group by Chapa, AnoCompetencia, MesCompetencia, CodEvento, DtPagto, Referencia, Valor, ValorOriginal, NPeriodo, Hora
";
        //where secao.taborg=5 and chapa.numcad = 359

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

            List<FichaFinanceira> fichas = new List<FichaFinanceira>();

            error = buscarFichasFinanceiras(fichas);

            FileHelperEngine engine = new FileHelperEngine(typeof(FichaFinanceira), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, fichas);
        }

        private bool buscarFichasFinanceiras(List<FichaFinanceira> fichas)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryFichas.Replace("{schemaName}", dbName));

            _bgWorker.ReportProgress(0, "Executando consulta...");

            IDataReader drFichas = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            _bgWorker.ReportProgress(0, "Montando objetos...");

            while (drFichas.Read())
            {
                FichaFinanceira ficha = new FichaFinanceira();

                try
                {
                    processedRecords++;

                    //ficha.Chapa = drFichas["Chapa"].ToString();
                    ficha.Chapa = drFichas["Chapa"].ToString();
                    ficha.Chapa = ficha.Chapa.PadLeft(5, '0');

                    ficha.AnoCompetencia = Convert.ToInt32(drFichas["AnoCompetencia"]);
                    ficha.MesCompetencia = Convert.ToInt32(drFichas["MesCompetencia"]);
                    ficha.CodEvento = drFichas["CodEvento"].ToString();
                    ficha.DtPagto = Convert.ToDateTime(drFichas["DtPagto"]);
                    ficha.Referencia = Convert.ToDouble(drFichas["Referencia"]);
                    ficha.Valor = Convert.ToDouble(drFichas["Valor"]);

                    ficha.NumPeriodo = Convert.ToInt32(drFichas["NPeriodo"]);//deixado fixo pois não foi encontrado no sistema atual.
                    ficha.ValorOriginal = Convert.ToDouble(drFichas["Valor"]);//mesmo campo valor, pois não foi encontrado no sistema atual.

                    fichas.Add(ficha);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a ficha financeira: Chapa {0}, Competência {1}/{2}. Motivo:{3}", ficha.Chapa,ficha.MesCompetencia.ToString(), ficha.AnoCompetencia.ToString(), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    }
}
