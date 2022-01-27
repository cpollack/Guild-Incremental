using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Dice
{
    private static readonly Regex numberToken = new Regex("^[0-9]+$");
    private static readonly Regex diceRollToken = new Regex("^([0-9]*)d([0-9]+|%)$");

    public static int Roll(string str)
    {
        int value = 0;

        var tokens = str.Replace("+", " + ").Replace("-", " - ").Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

        // Blank dice expressions end up being DiceExpression.Zero.
        if (!tokens.Any())
        {
            tokens = new[] { "0" };
        }

        // Since we parse tokens in operator-then-operand pairs, make sure the first token is an operand.
        if (tokens[0] != "+" && tokens[0] != "-")
        {
            tokens = (new[] { "+" }).Concat(tokens).ToArray();
        }

        // This is a precondition for the below parsing loop to make any sense.
        if (tokens.Length % 2 != 0)
        {
            Debug.LogError("Dice::Parse received an uneven number of arguments: " + str);
            return 0;
        }

        // Parse operator-then-operand pairs rolled values
        for (int tokenIndex = 0; tokenIndex < tokens.Length; tokenIndex += 2)
        {
            var token = tokens[tokenIndex];
            var nextToken = tokens[tokenIndex + 1];

            if (token != "+" && token != "-")
            {
                Debug.LogError("Dice::Parse The given dice expression was not in an expected format: " + str);
                return 0;
            }

            int multiplier = token == "+" ? +1 : -1;

            if (Dice.numberToken.IsMatch(nextToken))
            {
                value += multiplier * int.Parse(nextToken);
            }
            else if (Dice.diceRollToken.IsMatch(nextToken))
            {
                var match = Dice.diceRollToken.Match(nextToken);
                int numberOfDice = match.Groups[1].Value == string.Empty ? 1 : int.Parse(match.Groups[1].Value);
                int diceType = match.Groups[2].Value == "%" ? 100 : int.Parse(match.Groups[2].Value);

                int subTotal = 0;
                for (int i = 0; i < numberOfDice; i++)
                    subTotal += UnityEngine.Random.Range(1, diceType + 1);

                value += multiplier * subTotal;                
            }
            else
            {
                Debug.LogError("Dice::Parse The given dice expression was not in an expected format: the non-operand token was neither a number nor a dice-roll expression: " + str);
                return 0;
            }
        }

        return value;
    }
}
