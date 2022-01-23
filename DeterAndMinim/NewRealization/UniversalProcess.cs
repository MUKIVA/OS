using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRealization
{
    public class UniversalProcess
    {
        public void StartProcess(TextReader reader, TextWriter writer, IParseCellBehavior headerCellBehav, IParseCellBehavior dataCellBehav)
        {
            // Header
            string row = reader.ReadLine();
            var cellList = row.Split("\t", StringSplitOptions.RemoveEmptyEntries);
            List<List<Cell>> data = new();
            data.Add(new());
            for (int i = 0; i < cellList.Length; ++i)
            {
                data[0].Add(new Cell(i, cellList[i], headerCellBehav));
            }
            // Data
            cellList = null;
            while (reader.Peek() != -1)
            {
                row = reader.ReadLine();
                cellList = row.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                if (cellList.Length != 0 || cellList != null)
                    data.Add(new());
                for (int i = 0; i < cellList.Length; ++i)
                {
                    data[^1].Add(new Cell(i, cellList[i], dataCellBehav));
                }
                cellList = null;
            }
            GC.Collect();

            Table table = new Table(data, headerCellBehav, dataCellBehav);
            table.Minimize();
            table.PrintTable(writer);
        }
    }
}
