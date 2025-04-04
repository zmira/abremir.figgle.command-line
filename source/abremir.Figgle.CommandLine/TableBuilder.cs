// Based on https://github.com/Grizzly-pride/Console_Menu_Tools/blob/main/MenuTools/TableBuilder/TableBuilder.cs
using System.Text;
using static System.String;

namespace abremir.Figgle.CommandLine
{
    sealed class TableBuilder
    {
        #region Line items
        private const char TopLeftJoint = '┌';
        private const char TopRightJoint = '┐';
        private const char BottomLeftJoint = '└';
        private const char BottomRightJoint = '┘';
        private const char TopJoint = '┬';
        private const char BottomJoint = '┴';
        private const char HorizontalLine = '─';
        private const char VerticalLine = '│';
        #endregion

        private readonly int _columnsCount;
        private readonly int _columWidth;

        private readonly StringBuilder _sb = new();

        public TableBuilder(int columnsCount, int columnWidth)
        {
            _columnsCount = columnsCount;
            _columWidth = columnWidth;
            _sb.Capacity = Math.Abs(_columnsCount * _columWidth);
        }

        public string AddRow(params string[] items)
        {
            _sb.Clear();
            for (int item = 0; item < _columnsCount; item++)
            {
                var itemValue = item >= items.Length || items[item] is null
                    ? Empty
                    : items[item];
                string text = item switch
                {
                    int i when i == 0 => Concat(VerticalLine, itemValue.PadRight(_columWidth), VerticalLine),

                    _ => Concat(itemValue.PadRight(_columWidth), VerticalLine)
                };
                _sb.Append(text);
            }
            return _sb.ToString();
        }

        public string AddTopLine() => Line(TopLeftJoint, TopJoint, TopRightJoint);
        public string AddEndLine() => Line(BottomLeftJoint, BottomJoint, BottomRightJoint);

        private string Line(char leftJoint, char middleJoint, char rightJoint)
        {
            _sb.Clear();
            _sb.Capacity = Math.Abs(_columnsCount * _columWidth);

            for (int column = 0; column < _columnsCount; column++)
            {
                int size = Math.Abs(_columWidth);

                for (int joint = 0; joint <= size; joint++)
                {
                    if (column == 0)
                    {
                        switch (joint)
                        {
                            case 0: _sb.Append(leftJoint); break;
                            case int j when j == size: _sb.Append(HorizontalLine).Append(middleJoint); break;
                            default: _sb.Append(HorizontalLine); break;
                        }
                    }
                    else if (column == _columnsCount - 1)
                    {
                        switch (joint)
                        {
                            case int j when j == size: _sb.Append(rightJoint); break;
                            default: _sb.Append(HorizontalLine); break;
                        }
                    }
                    else
                    {
                        switch (joint)
                        {
                            case int j when j == size: _sb.Append(middleJoint); break;
                            default: _sb.Append(HorizontalLine); break;
                        }
                    }
                }
            }

            return _sb.ToString();
        }
    }
}