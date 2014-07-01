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
using System.Configuration;

namespace Exportador.RH.Ferias
{
    public class ExportadorVerbasFerias : IExportador
    {
        #region Private Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool _debugMode;

        #endregion

        #region Public Properties

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

        #region Queries

        private string _queryVerbasFerias = @"
select
	chapa.Chapa as 'CHAPA',
	REPLACE(CONVERT(char,periodo.fimper,103),'/','') as 'FIMPERAQUIS',
    REPLACE(CONVERT(char,recibo.datpag,103),'/','') as 'DATAPGTO',
case 
		when eventos.codeve = '58' or evento.deseve = '1/3 de Férias' then '0040'
when eventos.codeve = '1359' or evento.deseve = '1/3 de Férias' then '0040'
when eventos.codeve = '1379' or evento.deseve = '1/3 de Férias Resc.' then '0063'
when eventos.codeve = '67' or evento.deseve = '1/3 Férias Rescisäo' then '0063'
when eventos.codeve = '65' or evento.deseve = '13.º Indenizado Rescisão' then '0071'
when eventos.codeve = '50' or evento.deseve = '13º Salário Adiantado' then '0009'
when eventos.codeve = '46' or evento.deseve = '13º Salário Complementar' then '0066'
when eventos.codeve = '51' or evento.deseve = '13º Salário Integral' then '0102'
when eventos.codeve = '97' or evento.deseve = '13º Salário Prop.Rescisäo' then '0048'
when eventos.codeve = '57' or evento.deseve = 'Abono Pecuniário de férias' then '0039'
when eventos.codeve = '1827' or evento.deseve = 'ACORDO JUDICIAL/EXTA JUD' then '1035'
when eventos.codeve = '270' or evento.deseve = 'Adic.Noturno 13º Inden.' then '9007'
when eventos.codeve = '110' or evento.deseve = 'Adic.Noturno A.P.I' then '1028'
when eventos.codeve = '273' or evento.deseve = 'Adic.Noturno API' then '1028'
when eventos.codeve = '27' or evento.deseve = 'Adicional Noturno' then '0405'
when eventos.codeve = '1382' or evento.deseve = 'ADICIONAL NOTURNO API' then '1028'
when eventos.codeve = '1330' or evento.deseve = 'ADICIONAL NOTURNO PROF.' then '1025'
when eventos.codeve = '428' or evento.deseve = 'ADUNIPLAC MENSALIDADE' then '1038'
when eventos.codeve = '1826' or evento.deseve = 'AJUDA EXTRA_CLASSE' then '1034'
when eventos.codeve = '414' or evento.deseve = 'AULAS ENS SUP C/DSR(V)' then '1005'
when eventos.codeve = '527' or evento.deseve = 'Auxilio Funeral' then '1058'
when eventos.codeve = '69' or evento.deseve = 'Aviso Prévio Indenizado' then '62'
when eventos.codeve = '1380' or evento.deseve = 'Aviso Prévio Indenizado' then '0062'
when eventos.codeve = '113' or evento.deseve = 'Aviso Prévio Trab.Noturno' then '62'
when eventos.codeve = '1828' or evento.deseve = 'BOLSA DE PESQUISA' then '1036'
when eventos.codeve = '1830' or evento.deseve = 'Cerimoniais' then '1065'
when eventos.codeve = '1028' or evento.deseve = 'CLINICA INTEGRADA ODONTO' then '1008'
when eventos.codeve = '1829' or evento.deseve = 'Comissão comercial' then '1060'
when eventos.codeve = '610' or evento.deseve = 'COMPLEMENTO DE SALÁRIO' then '1006'
when eventos.codeve = '141' or evento.deseve = 'CONT.CONF.CLAUS.CCT/SAAER' then '1069'
when eventos.codeve = '148' or evento.deseve = 'Contrib. Sindical Horista' then '0012'
when eventos.codeve = '854' or evento.deseve = 'CONTRIBUIÇÃO CONFEDERATIV' then '1067'
when eventos.codeve = '147' or evento.deseve = 'Contribuiçäo Sindical' then '0012'
when eventos.codeve = '959' or evento.deseve = 'desc. 13 sala. adiant' then '0035'
when eventos.codeve = '156' or evento.deseve = 'DESC. ADIANT.SALARIAL' then '0019'
when eventos.codeve = '1820' or evento.deseve = 'DESC. ADUNIPLAC DIVERSOS' then '1043'
when eventos.codeve = '948' or evento.deseve = 'DESC. DIFERENCA SALARIAL' then '1079'
when eventos.codeve = '1221' or evento.deseve = 'DESC.BANCO BANRISUL (Consignado)' then '0036'
when eventos.codeve = '846' or evento.deseve = 'DESC.SALARIO PAGO MAIOR' then '1061'
when eventos.codeve = '1430' or evento.deseve = 'Desconto 13ºSal.Adiantado (Professor)' then '0035'
when eventos.codeve = '138' or evento.deseve = 'Desconto Adto Férias' then '0043'
when eventos.codeve = '427' or evento.deseve = 'DESCONTO AFEUP' then '1037'
when eventos.codeve = '839' or evento.deseve = 'DESCONTO AUTORIZADO' then '1039'
when eventos.codeve = '1226' or evento.deseve = 'DESCONTO BANCO BRASIL' then '1053'
when eventos.codeve = '1085' or evento.deseve = 'DESCONTO SOCIO SAAERS' then '1041'
when eventos.codeve = '1133' or evento.deseve = 'DESCONTO SOCIO SINPROESC' then '1042'
when eventos.codeve = '1061' or evento.deseve = 'DESCONTO TELEFONE' then '1040'
when eventos.codeve = '686' or evento.deseve = 'Dev. Desconto Indevido' then '1063'
when eventos.codeve = '1825' or evento.deseve = 'Dev.Desc.a maior Férias' then '1080'
when eventos.codeve = '1824' or evento.deseve = 'Dif. Triênio mês anterior' then '1064'
when eventos.codeve = '35' or evento.deseve = 'Dif.Salario Mes Anterior' then '0037'
when eventos.codeve = '113' or evento.deseve = 'Dif.triênio 3% pg.a menor' then '1057'
when eventos.codeve = '60' or evento.deseve = 'Diferença de Férias' then '0045'
when eventos.codeve = '973' or evento.deseve = 'Diferenca de Maternidade' then '1086'
when eventos.codeve = '270' or evento.deseve = 'Diferença de Salário' then '37'
when eventos.codeve = '139' or evento.deseve = 'Diferença IRRF Férias' then '1056'
when eventos.codeve = '42' or evento.deseve = 'DSR (VALOR)' then '1003'
when eventos.codeve = '1813' or evento.deseve = 'DSR HORAS PROFICIËNCIA' then '1031'
when eventos.codeve = '45' or evento.deseve = 'DSR s/ Horas Extras' then '0404'
when eventos.codeve = '141' or evento.deseve = 'Estabilidade Rescisäo' then '1072'
when eventos.codeve = '191' or evento.deseve = 'Estouro do Mês' then '0020'
when eventos.codeve = '190' or evento.deseve = 'Estouro Mês Anterior' then '0021'
when eventos.codeve = '78' or evento.deseve = 'Férias Proporc.Rescisäo' then '0025'
when eventos.codeve = '178' or evento.deseve = 'Ferias S/Rescisao' then '0025'
when eventos.codeve = '66' or evento.deseve = 'Férias Vencidas Rescisäo' then '24'
when eventos.codeve = '110' or evento.deseve = 'FGTS' then '1089'
when eventos.codeve = '117' or evento.deseve = 'FGTS 13. SAL. RESCISAO' then '31'
when eventos.codeve = '136' or evento.deseve = 'FGTS 13. SALARIO' then '31'
when eventos.codeve = '73' or evento.deseve = 'FGTS 40% Rescisäo (Valor)' then '28'
when eventos.codeve = '72' or evento.deseve = 'FGTS Rescisäo (Valor)' then '26'
when eventos.codeve = '1453' or evento.deseve = 'Gratif. Aux. Maternidade' then '1047'
when eventos.codeve = '875' or evento.deseve = 'GRATIFICAÇÃO DE FUNÇÃO' then '1007'
when eventos.codeve = '1823' or evento.deseve = 'H.PROC.IMPL.OP.BCO DENTES' then '1033'
when eventos.codeve = '1452' or evento.deseve = 'HORAS AF.AUX.MATERNIDADE' then '1029'
when eventos.codeve = '24' or evento.deseve = 'Horas Afast.Acid.Trabalho' then '1084'
when eventos.codeve = '1451' or evento.deseve = 'Horas Afast.Aux.Doenca' then '1088'
when eventos.codeve = '25' or evento.deseve = 'Horas Afast.Aux.Doença' then '1088'
when eventos.codeve = '1313' or evento.deseve = 'HORAS ASSESSORES' then '1019'
when eventos.codeve = '16' or evento.deseve = 'HORAS AUX.PATERNIDADE' then '1002'
when eventos.codeve = '12' or evento.deseve = 'HORAS AUXÍLIO MATERNIDADE' then '0017'
when eventos.codeve = '1821' or evento.deseve = 'HORAS COMISSÕES ANÁLISE' then '1032'
when eventos.codeve = '1500' or evento.deseve = 'Horas Controle de Jornada' then '1082'
when eventos.codeve = '1307' or evento.deseve = 'HORAS COORDENAÇÃO CURSO' then '1014'
when eventos.codeve = '1320' or evento.deseve = 'HORAS DSR' then '1022'
when eventos.codeve = '1321' or evento.deseve = 'HORAS DSR COORD/MEST/CHEF' then '1023'
when eventos.codeve = '1358' or evento.deseve = 'HORAS DSR FÉRIAS' then '1027'
when eventos.codeve = '1818' or evento.deseve = 'Horas Ens Sup FUMDES BR' then '1062'
when eventos.codeve = '1822' or evento.deseve = 'Horas Ens Sup FUMDES Lg' then '1068'
when eventos.codeve = '1816' or evento.deseve = 'Horas Ens Sup FUMDES SJ' then '1071'
when eventos.codeve = '1819' or evento.deseve = 'Horas Ens Sup Intensivo' then '1081'
when eventos.codeve = '1300' or evento.deseve = 'HORAS ENSINO SUPERIOR' then '1009'
when eventos.codeve = '1314' or evento.deseve = 'HORAS ENSINO SUPERIOR' then '1020'
when eventos.codeve = '21' or evento.deseve = 'Horas Extras c/ 50%' then '0406'
when eventos.codeve = '23' or evento.deseve = 'Horas Extras Noturna 50%' then '1078'
when eventos.codeve = '1510' or evento.deseve = 'Horas Falta' then '8'
when eventos.codeve = '23' or evento.deseve = 'Horas Faltas DSR Noturno' then '1087'
when eventos.codeve = '7' or evento.deseve = 'Horas Ferias Diurnas' then '0041'
when eventos.codeve = '8' or evento.deseve = 'Horas Férias Noturnas' then '0041'
when eventos.codeve = '1312' or evento.deseve = 'HORAS GESTOR' then '1018'
when eventos.codeve = '1306' or evento.deseve = 'Horas Mestre' then '1070'
when eventos.codeve = '1' or evento.deseve = 'Horas Normais' then '0001'
when eventos.codeve = '2' or evento.deseve = 'Horas Normais Noturnas' then '0001'
when eventos.codeve = '1309' or evento.deseve = 'HORAS ORIENT.EST/TCC/MON' then '1016'
when eventos.codeve = '1812' or evento.deseve = 'HORAS PROFICIÊNCIAS' then '1030'
when eventos.codeve = '1302' or evento.deseve = 'HORAS PROJETO EXTENSÃO' then '1011'
when eventos.codeve = '1303' or evento.deseve = 'HORAS PROJETO PESQUISA' then '1012'
when eventos.codeve = '1305' or evento.deseve = 'HORAS PROJETO PG MESTRADO' then '1013'
when eventos.codeve = '3' or evento.deseve = 'HORAS REPOUSO REM.DIURNO' then '1001'
when eventos.codeve = '1317' or evento.deseve = 'Horas Reunioes/Conferenc.' then '1055'
when eventos.codeve = '1311' or evento.deseve = 'HORAS SECRETÁRIOS' then '1017'
when eventos.codeve = '25' or evento.deseve = 'Horas Ser.Militar Noturna' then '1083'
when eventos.codeve = '24' or evento.deseve = 'Horas Serviço Militar' then '1085'
when eventos.codeve = '1308' or evento.deseve = 'HORAS SUPERV. EST/TCC/MON' then '1015'
when eventos.codeve = '1832' or evento.deseve = 'Horas tradução linguas' then '1075'
when eventos.codeve = '1318' or evento.deseve = 'Horas Turmas Especiais' then '1073'
when eventos.codeve = '1806' or evento.deseve = 'Hs Ates.Med. ate 15dias N' then '0145'
when eventos.codeve = '14' or evento.deseve = 'Hs Ates.Medico ate 15dias' then '0145'
when eventos.codeve = '1319' or evento.deseve = 'HS BANCAS EST/TCC/MON/PRO' then '1021'
when eventos.codeve = '1506' or evento.deseve = 'Ind. Térm. Contr. Antec.' then '0124'
when eventos.codeve = '102' or evento.deseve = 'IND.ART.9º LEI 6708' then '1000'
when eventos.codeve = '1805' or evento.deseve = 'Indenização' then '1076'
when eventos.codeve = '72' or evento.deseve = 'Insalubridade 13º Integ.' then '1092'
when eventos.codeve = '66' or evento.deseve = 'Insalubridade s/ Férias' then '1093'
when eventos.codeve = '120' or evento.deseve = 'INSS' then '0003'
when eventos.codeve = '122' or evento.deseve = 'INSS s/ 13º Salário' then '0011'
when eventos.codeve = '124' or evento.deseve = 'INSS s/ Férias' then '0082'
when eventos.codeve = '130' or evento.deseve = 'IRRF' then '0004'
when eventos.codeve = '132' or evento.deseve = 'IRRF s/ 13º Salário' then '0049'
when eventos.codeve = '134' or evento.deseve = 'IRRF s/ Férias' then '0030'
when eventos.codeve = '32' or evento.deseve = 'LICENÇA REMUNERADA' then '0424'
when eventos.codeve = '105' or evento.deseve = 'Líquido Rescisäo' then '0399'
when eventos.codeve = '202' or evento.deseve = 'Média H.Extra 13º Inden.' then '9017'
when eventos.codeve = '199' or evento.deseve = 'Média H.Extra 13º Prop.Re' then '9017'
when eventos.codeve = '205' or evento.deseve = 'Média H.Extra Férias Resc' then '9012'
when eventos.codeve = '202' or evento.deseve = 'Média H.Extras 13º Adto' then '9015'
when eventos.codeve = '117' or evento.deseve = 'Média H.Extras Grat.Resc.' then '1091'
when eventos.codeve = '86' or evento.deseve = 'Média Hs.Extra API' then '1090'
when eventos.codeve = '203' or evento.deseve = 'Média Variáv. 13º Inden.' then '9048'
when eventos.codeve = '1401' or evento.deseve = 'Média Variáv.13ºInd.Resc' then '9018'
when eventos.codeve = '87' or evento.deseve = 'Média Variáveis API' then '1095'
when eventos.codeve = '1381' or evento.deseve = 'Média Variáveis API' then '1095'
when eventos.codeve = '136' or evento.deseve = 'Média Variáveis Férias' then '9041'
when eventos.codeve = '119' or evento.deseve = 'Pensäo Judicial' then '0013'
when eventos.codeve = '186' or evento.deseve = 'Pensäo Judicial 13º Sal.' then '0120'
when eventos.codeve = '396' or evento.deseve = 'Pensäo Judicial s/Ferias' then '0113'
when eventos.codeve = '1999' or evento.deseve = 'Período Sem Atividade' then '1077'
when eventos.codeve = '1062' or evento.deseve = 'PIS na Folha (abono/Rend)' then '1054'
when eventos.codeve = '1301' or evento.deseve = 'PÓS-GRADUAÇÃO' then '1010'
when eventos.codeve = '1831' or evento.deseve = 'Projeto de Extensão ' then '1074'
when eventos.codeve = '325' or evento.deseve = 'REP.VESTIBULAR/CONCURSOS' then '1004'
when eventos.codeve = '1336' or evento.deseve = 'RES.MEDICA MIN.SAÚDE' then '1026'
when eventos.codeve = '1337' or evento.deseve = 'Res.Multiprofissional' then '1066'
when eventos.codeve = '1324' or evento.deseve = 'SALÁRIO ADM' then '1024'
when eventos.codeve = '31' or evento.deseve = 'Salario Familia' then '0005'
when eventos.codeve = '101' or evento.deseve = 'Saldo de Salário' then '0401'
when eventos.codeve = '131' or evento.deseve = 'SUPERV.ESTAGIO PSICOLOGIA' then '1059'
when eventos.codeve = '1550' or evento.deseve = 'Triênio' then '0403'
when eventos.codeve = '205' or evento.deseve = 'Triênio 13º Adto' then '1049'
when eventos.codeve = '1409' or evento.deseve = 'Triênio 13o Sal.Ind.Resc' then '1052'
when eventos.codeve = '1399' or evento.deseve = 'Triênio 13o Sal.Prop.Resc' then '1052'
when eventos.codeve = '1389' or evento.deseve = 'Triênio A.P.I' then '1094'
when eventos.codeve = '1551' or evento.deseve = 'Triênio s/ Férias ' then '1048'
when eventos.codeve = '1377' or evento.deseve = 'Triênio s/ Férias Resc.' then '1048'
when eventos.codeve = '152' or evento.deseve = 'Vale Transporte' then '0006'
when eventos.codeve = '1400' or evento.deseve = '13º Sal.Ind.Resc.Méd.Hor' then '9068'
when eventos.codeve = '1390' or evento.deseve = '13º Sal.Prop.Resc.Méd.Hor' then '9068'
when eventos.codeve = '1410' or evento.deseve = '13ºSal.Adiant.Méd.Horas' then '9066'
when eventos.codeve = '1425' or evento.deseve = '13ºSal.Int.Matern.Med.Hor' then '9071'
when eventos.codeve = '1420' or evento.deseve = '13ºSal.Integal Méd.Horas' then '9067'
when eventos.codeve = '1392' or evento.deseve = 'Ad.Not.13º Sal.Prop.Resc.' then '9007'
when eventos.codeve = '1402' or evento.deseve = 'Ad.Noturno 13ºInd.Resc' then '9007'
when eventos.codeve = '1373' or evento.deseve = 'Adic.Noturno Férias Resc.' then '9002'
when eventos.codeve = '1353' or evento.deseve = 'Adic.Noturno s/Férias' then '9001'
when eventos.codeve = '1408' or evento.deseve = 'Horas DSR 13ºInd.Resc.' then '9037'
when eventos.codeve = '1398' or evento.deseve = 'Horas DSR 13ºProp.Resc.' then '9037'
when eventos.codeve = '1388' or evento.deseve = 'Horas DSR API' then '9082'
when eventos.codeve = '1378' or evento.deseve = 'Horas DSR Férias Resc.' then '9032'
when eventos.codeve = '1418' or evento.deseve = 'Horas DSR s/13ºSal.Adiant' then '9035'
when eventos.codeve = '1428' or evento.deseve = 'Horas DSR s/13ºSal.Integ' then '9036'
when eventos.codeve = '1350' or evento.deseve = 'Horas Férias - Médias' then '9062'
when eventos.codeve = '1371' or evento.deseve = 'Horas Férias Prop. Médias' then '9064'
when eventos.codeve = '1370' or evento.deseve = 'Horas Férias Venc. Médias' then '9063'
when eventos.codeve = '1391' or evento.deseve = 'Média Variáv.13ºProp.Resc' then '9078'
when eventos.codeve = '1372' or evento.deseve = 'Médias Férias Vlr.Vár' then '9041'
when eventos.codeve = '1352' or evento.deseve = 'Médias Férias Vlr.Vár.' then '9072'
when eventos.codeve = '1556' or evento.deseve = 'Triênio 13 Sal.Adto.' then '1049'
when eventos.codeve = '964' or evento.deseve = '13 Salario Media Valores' then '9079'
when eventos.codeve = '106' or evento.deseve = '13º Sal./Aux.Maternidade' then '9081'
when eventos.codeve = '1411' or evento.deseve = '13ºSal.Adiant.Méd.Vlr.Var' then '9076'
when eventos.codeve = '1421' or evento.deseve = '13ºSal.Integ.Méd.Vlr.Var' then '9077'
when eventos.codeve = '261' or evento.deseve = 'Adic.Noturno 13º Adto' then '9005'
when eventos.codeve = '63' or evento.deseve = 'Adic.Noturno 13º Salário' then '9006'
when eventos.codeve = '1557' or evento.deseve = 'Triênio 13o Sal.Int.' then '1050'
when eventos.codeve = '1560' or evento.deseve = 'Trienio Maternidade' then '1051'
when eventos.codeve = '1354' or evento.deseve = 'Triênio s/ Férias' then '1048'
when eventos.codeve = '266' or evento.deseve = 'Adic.Noturno 13º Proporc.' then '9007'
when eventos.codeve = '265' or evento.deseve = 'Adic.Noturno Férias Resc.' then '9002'
when eventos.codeve = '262' or evento.deseve = 'Adic.Noturno s/Férias' then '9001'
when eventos.codeve = '206' or evento.deseve = 'Férias Media Rescisao' then '9073'
when eventos.codeve = '1227' or evento.deseve = 'Gratificação 13º Adiantado' then '1045'
when eventos.codeve = '287' or evento.deseve = 'Gratificaçäo 13º Integral' then '1046'
when eventos.codeve = '1817' or evento.deseve = 'Gratificação Férias' then '1044'
when eventos.codeve = '179' or evento.deseve = 'Med.Hrs.13º Sal.Prop.Resc' then '9068'
when eventos.codeve = '258' or evento.deseve = 'Média Hs.Extra 13º Adto' then '9015'
when eventos.codeve = '48' or evento.deseve = 'Média Hs.Extra s/13º Sal.' then '9016'
when eventos.codeve = '54' or evento.deseve = 'Média Hs.Extra s/Férias' then '9021'
when eventos.codeve = '200' or evento.deseve = 'Média Variáv.13º Prop.Res' then '9048'
when eventos.codeve = '55' or evento.deseve = 'Média Vlr.Var. s/Férias' then '9041'

	end	as 'CodEvento',
	'' AS 'HORA',
	CAST(eventos.refeve AS MONEY) as 'REF',
	CAST(eventos.valeve AS MONEY) as 'VALOR',
	0 as 'ALTERADOMANUAL'
--select *
from vetorh.r040fev eventos
					inner join dbo.vw_totvs_chapafuncionario chapa on eventos.numcad = chapa.numcad
																   and eventos.tipcol = chapa.tipcol
					inner join vetorh.r040fem recibo on eventos.numemp = recibo.numemp
													and eventos.tipcol = recibo.tipcol
													and eventos.numcad = recibo.numcad
													and eventos.iniper = recibo.iniper
													and eventos.inifer = recibo.inifer
					inner join vetorh.r040per periodo on recibo.numemp = periodo.numemp
													and recibo.tipcol = periodo.tipcol
													and recibo.numcad = periodo.numcad
													and recibo.iniper = periodo.iniper	
inner join vetorh.r008evc evento on eventos.codeve = evento.codeve																		
where cast(periodo.iniper AS DATE) >= '2012-01-01 00:00:00.000' and chapa.numcpf <> '0'";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarVerbasFerias();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorVerbasFerias()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorVerbasFerias(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorVerbasFerias(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarVerbasFerias()
        {
            List<VerbasFerias> periodos = new List<VerbasFerias>();

            periodos.AddRange(buscarVerbasFerias());

            FileHelperEngine engine = new FileHelperEngine(typeof(VerbasFerias), Encoding.UTF8);

            engine.WriteFile(_filename, periodos);
        }

        private List<VerbasFerias> buscarVerbasFerias()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryVerbasFerias.Replace("{schemaName}", dbName));

            IDataReader periodos = database.ExecuteReader(command);

            List<VerbasFerias> lverbas = new List<VerbasFerias>();

            while (periodos.Read())
            {
                VerbasFerias lnverbas = new VerbasFerias();

                lnverbas.CHAPA = periodos["CHAPA"].ToString();
                lnverbas.FIMPERAQUIS = periodos["FIMPERAQUIS"].ToString();
                lnverbas.DATAPGTO = periodos["DATAPGTO"].ToString();
                lnverbas.CodEvento = periodos["CodEvento"].ToString();
                lnverbas.HORA = periodos["HORA"].ToString();
                lnverbas.REF = periodos["REF"].ToString();
                lnverbas.VALOR = periodos["VALOR"].ToString();
                lnverbas.ALTERADOMANUAL = periodos["ALTERADOMANUAL"].ToString();

                lverbas.Add(lnverbas);
            }

            return lverbas;
        }
    }
}
