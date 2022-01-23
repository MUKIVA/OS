using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRealization
{
    public class Table
    {
        private List<List<Cell>> _data;
        public IParseCellBehavior headerParser;
        public IParseCellBehavior dataParser;

        public Table(List<List<Cell>> data, IParseCellBehavior headerParser, IParseCellBehavior dataParser)
        {
            _data = data;
            this.headerParser = headerParser;
            this.dataParser = dataParser;
        }

        public List<List<Cell>> Data
        {
            get => _data;
        }
        public List<Cell> GetRow(int index) => _data[index];
        public List<Cell> GetColumn(int index) => _data.Select(row => row[index]).ToList();
        public Cell GetCell(int row, int column) => _data[row][column];

        public void Minimize()
        {
            // Получаем классы эквивалентности
            var oldClasses = GetEqualClasses(this);
            // Строим новые таблицы
            while (true)
            { 
                var intermediateTable = CreateIntermediateTable(this, oldClasses);
                var newClasses = GetIntermediateEqualCalsses(intermediateTable, oldClasses);
                if (newClasses.Count == oldClasses.Count) break;
                oldClasses = newClasses;
            }
            // Обновляем текущую таблицу
            _data = CreateMinimizeTable(this, oldClasses).Data;
        }

        public void PrintTable(TextWriter writer)
        {
            foreach (var row in _data)
            {
                foreach (var cell in row)
                    cell.Print(writer);
                writer.WriteLine();
            }
        }

        public static Dictionary<string, List<Cell>> GetEqualClasses(Table table, Dictionary<string, List<Cell>> oldClasses = null)
        {
            Dictionary<string, List<Cell>> _ = new();
            HashSet<int> indexForSkip = new();
            for (int i = 0; i < table.GetRow(0).Count; ++i)
            {
                if (indexForSkip.Contains(i)) continue;
                var column = table.GetColumn(i);
                var comboKey = "";
                column.ForEach(cell => comboKey += cell.Enter);
                _.Add(comboKey, new());
                for (int j = i; j < table.GetRow(0).Count; ++j)
                {
                    if (indexForSkip.Contains(j)) continue;
                    var anotherColumn = table.GetColumn(j);
                    var anotherComboKey = "";
                    anotherColumn.ForEach(cell => anotherComboKey += cell.Enter);
                    if (anotherComboKey == comboKey)
                    {
                        _[anotherComboKey].Add(anotherColumn[0]);
                        indexForSkip.Add(j);
                    }
                }
            }
            return _;
        }

        public static Table CreateMinimizeTable(Table table, Dictionary<string, List<Cell>> classes)
        {
            List<List<Cell>> data = new();
            // Header
            data.Add(new());
            classes.ToList().ForEach(@class => { data[0].Add(@class.Value[0]); });
            // Data
            Dictionary<string, string> _ = new();
            classes.ToList().ForEach(@class => @class.Value.ForEach(cell => _.Add(cell.State, @class.Key)));
            foreach (var row in table.Data.Skip(1))
            {
                data.Add(new());
                data[0].ForEach(cell =>
                {
                    var column = table.GetColumn(cell.Index);
                    data[^1].Add(new Cell(
                        cell.Index,
                        $"{classes[_[column[data.Count - 1].State]][0].State}/{column[data.Count - 1].Enter}",
                        table.dataParser));
                });
            }
            return new Table(data, table.headerParser, table.dataParser);
        }

        private static Table CreateIntermediateTable(Table table, Dictionary<string, List<Cell>> classes)
        {
            List<List<Cell>> data = new();
            // Header
            data.Add(table.GetRow(0));
            Dictionary<string, string> _ = new();
            classes.ToList().ForEach(@class => @class.Value.ForEach(cell => _.Add(cell.State, @class.Key)));
            // Data
            table.Data.Skip(1).ToList().ForEach(
                row =>
                {
                    data.Add(new());
                    row.ForEach(el =>
                    {
                        data[^1]
                        .Add(new Cell(el.Index, _[el.State], new SingleCellBehaviour()));
                    });
                });


            return new Table(data, table.headerParser, new SingleCellBehaviour());
        }

        private static Dictionary<string, List<Cell>> GetIntermediateEqualCalsses(Table table, Dictionary<string, List<Cell>> oldClasses = null)
        {
            if (oldClasses == null) return null;
            Dictionary<string, List<Cell>> _ = new();

            foreach (var @class in oldClasses)
            {
                foreach (Cell cell in @class.Value)
                {
                    var column = table.GetColumn(cell.Index);
                    var comboKey = @class.Key;
                    column.Skip(1).ToList().ForEach(key =>
                    {
                        comboKey += key.State;
                    });

                    if (_.ContainsKey(comboKey))
                        _[comboKey].Add(cell);
                    else
                        _.Add(comboKey, new List<Cell>() { cell });
                }
            }
            return _;
        }


    }
}
