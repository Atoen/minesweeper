﻿using System.Collections.ObjectModel;

namespace Minesweeper.UI;

public static class GridLines
{
    public static readonly ReadOnlyDictionary<GridLineStyle, Dictionary<GridLineFragment, char>> Symbols = new(
        new Dictionary<GridLineStyle, Dictionary<GridLineFragment, char>>
        {
            {
                GridLineStyle.Single, new Dictionary<GridLineFragment, char>
                {
                    {GridLineFragment.Vertical, '│'},
                    {GridLineFragment.Horizontal, '─'},
                    {GridLineFragment.HorizontalDown, '┬'},
                    {GridLineFragment.HorizontalUp, '┴'},
                    {GridLineFragment.VerticalRight, '├'},
                    {GridLineFragment.VerticalLeft, '┤'},
                    {GridLineFragment.Cross, '┼'},
                }
            },

            {
                GridLineStyle.SingleBold, new Dictionary<GridLineFragment, char>
                {
                    {GridLineFragment.Vertical, '┃'},
                    {GridLineFragment.Horizontal, '━'},
                    {GridLineFragment.HorizontalDown, '┳'},
                    {GridLineFragment.HorizontalUp, '┻'},
                    {GridLineFragment.VerticalRight, '┣'},
                    {GridLineFragment.VerticalLeft, '┫'},
                    {GridLineFragment.Cross, '╋'},
                }  
            },

            {
                GridLineStyle.Double, new Dictionary<GridLineFragment, char>
                {
                    {GridLineFragment.Vertical, '║'},
                    {GridLineFragment.Horizontal, '═'},
                    {GridLineFragment.HorizontalDown, '╦'},
                    {GridLineFragment.HorizontalUp, '╩'},
                    {GridLineFragment.VerticalRight, '╠'},
                    {GridLineFragment.VerticalLeft, '╣'},
                    {GridLineFragment.Cross, '╬'},
                }
            },

            {
                GridLineStyle.Dashed, new Dictionary<GridLineFragment, char>
                {
                    {GridLineFragment.Vertical, '╎'},
                    {GridLineFragment.Horizontal, '-'},
                    {GridLineFragment.HorizontalDown, '·'},
                    {GridLineFragment.HorizontalUp, '·'},
                    {GridLineFragment.VerticalRight, '·'},
                    {GridLineFragment.VerticalLeft, '·'},
                    {GridLineFragment.Cross, '·'},
                }
            }
        });

}

public enum GridLineStyle
{
    Single,
    Double,
    SingleBold,
    Dashed
}

public enum GridLineFragment
{
    Vertical,
    VerticalLeft,
    VerticalRight,
    Horizontal,
    HorizontalUp,
    HorizontalDown,
    Cross
}
