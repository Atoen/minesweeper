﻿using System.Collections.ObjectModel;

namespace Minesweeper.UI;

public static class Border
{
    public static readonly ReadOnlyDictionary<BorderStyle, Dictionary<BorderFragment, char>> Symbols = new(
        new Dictionary<BorderStyle, Dictionary<BorderFragment, char>>
        {
            {
                BorderStyle.Single, new Dictionary<BorderFragment, char>
                {
                    {BorderFragment.Vertical, '│'},
                    {BorderFragment.Horizontal, '─'},
                    {BorderFragment.UpperLeft, '┌'},
                    {BorderFragment.UpperRight, '┐'},
                    {BorderFragment.LowerLeft, '└'},
                    {BorderFragment.LowerRight, '┘'}
                }
            },

            {
                BorderStyle.Double, new Dictionary<BorderFragment, char>
                {
                    {BorderFragment.Vertical, '║'},
                    {BorderFragment.Horizontal, '═'},
                    {BorderFragment.UpperLeft, '╔'},
                    {BorderFragment.UpperRight, '╗'},
                    {BorderFragment.LowerLeft, '╚'},
                    {BorderFragment.LowerRight, '╝'}
                }
            },

            {
                BorderStyle.Rounded, new Dictionary<BorderFragment, char>
                {
                    {BorderFragment.Vertical, '│'},
                    {BorderFragment.Horizontal, '─'},
                    {BorderFragment.UpperLeft, '╭'},
                    {BorderFragment.UpperRight, '╮'},
                    {BorderFragment.LowerLeft, '╰'},
                    {BorderFragment.LowerRight, '╯'}
                }
            },

            {
                BorderStyle.Dashed, new Dictionary<BorderFragment, char>
                {
                    {BorderFragment.Vertical, '┆'},
                    {BorderFragment.Horizontal, '╌'},
                    {BorderFragment.UpperLeft, '┌'},
                    {BorderFragment.UpperRight, '┐'},
                    {BorderFragment.LowerLeft, '└'},
                    {BorderFragment.LowerRight, '┘'}
                }
            },
            
            {
                BorderStyle.Dotted, new Dictionary<BorderFragment, char>
                {
                    {BorderFragment.Vertical, '╎'},
                    {BorderFragment.Horizontal, '-'},
                    {BorderFragment.UpperLeft, '·'},
                    {BorderFragment.UpperRight, '·'},
                    {BorderFragment.LowerLeft, '·'},
                    {BorderFragment.LowerRight, '·'}
                }
            }
        });
}

public enum BorderStyle
{
    Single,
    Double,
    Rounded,
    Dashed,
    Dotted
}

public enum BorderFragment
{
    Vertical,
    Horizontal,
    UpperLeft,
    UpperRight,
    LowerLeft,
    LowerRight
}
