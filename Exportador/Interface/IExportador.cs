using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportador.Interface
{
    public interface IExportador
    {
        void Exportar();

        /// <summary>
        /// Número de registros a ser retornado por iteração.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Número de registros totais a ser retornados no processo.
        /// 0 para todos.
        /// </summary>
        int RecordsCount { get; set; }

        /// <summary>
        /// Parâmetro para informar se o importar será executado em modo Debug.
        /// </summary>
        bool DebugMode { get; set; }
    }
}
