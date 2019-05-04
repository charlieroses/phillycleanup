
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DefaultNamespace;
using UnityEngine;

public class Lexer
{
    private Dictionary<string, Token.TokenType> Keywords;

    public string Source { get; set; }
    public int Start;
    public int Current;
    public List<Token> Tokens;

    public Lexer(string expr)
    {
        Source = expr;

        Start = 0;
        Current = 0;

        Tokens = new List<Token>();
        
        // add keywords
        Keywords = new Dictionary<string, Token.TokenType>();
            Keywords.Add("repeat", Token.TokenType.REPEAT);
            Keywords.Add("if", Token.TokenType.IF);
            Keywords.Add("else", Token.TokenType.ELSE);
            Keywords.Add("macro", Token.TokenType.MACRO);
            Keywords.Add("do", Token.TokenType.DO);
            Keywords.Add("endm", Token.TokenType.ENDM);
    }
    
    public class Token
    {
        public enum TokenType
        {
            NUMBER, IDENTIFIER,
            PLUS, MINUS, MULTIPLY, DIVIDE,
            EQUAL,
            
            REPEAT, COLON, IF, ELSE, 
            HASH, MACRO, EOF, DO, ENDM
        };

        public TokenType Type;
        public System.Object Value;
        public string Literal;

        public string ToString()
        {
            return $"Token [{Type}]: {Literal}";
        }
    }

    public List<Token> Scan()
    {
        while (!isAtEnd())
        {
            Start = Current;
            scanToken();
        }
        
        addToken(Token.TokenType.EOF, "");
        return Tokens;
    }

    void scanToken()
    {
        char c = advance();

        switch (c)
        {
            // nothing to see here, move along
            case ' ':
            case '\r':
            case '\t':
            case '\n':
                break;
            
            // single character tokens
            case '+':
                addToken(Token.TokenType.PLUS, "+");
                break;
            
            case '-':
                addToken(Token.TokenType.MINUS, "-");
                break;
                
            case '*':
                addToken(Token.TokenType.MULTIPLY, "*");
                break;
            
            case '/':
                addToken(Token.TokenType.DIVIDE, "/");
                break;
            
            case '#':
                addToken(Token.TokenType.HASH, "#");
                break;
            
            case ':':
                addToken(Token.TokenType.COLON, ":");
                break;
            
            case '=':
                addToken(Token.TokenType.EQUAL, "=");
                break;
            
            default:
                if (isDigit(c))
                {
                    number();
                } else if (isAlpha(c))
                {
                    identifier();
                }
                else
                {
                    throw new ScanError("Invalid character found in input!");
                }
                break;
        }
    }

    void addToken(Token.TokenType toAdd, string lit, System.Object val = null)
    {
        Tokens.Add(new Token{Type = toAdd, Value = val, Literal = lit});
    }

    void number()
    {
        while (isDigit(peek()))
        {
            advance();
        }
        
        addToken(Token.TokenType.NUMBER, Source.Substring(Start, Current - Start));
    }

    void identifier()
    {
        var c = peek();
        while (isAlphaNumeric(c))
        {
            advance();
            c = peek();
        }

        string text = Source.Substring(Start, Current - Start);
        if (Keywords.ContainsKey(text))
        {
            addToken(Keywords[text], text);
        }
        else
        {
            addToken(Token.TokenType.IDENTIFIER, text);
        }
    }

    char advance()
    {
        return Source[Current++];
    }

    char peek()
    {
        if (isAtEnd())
        {
            return '\0';
        }

        return Source[Current];
    }

    bool isDigit(char c)
    {
        return (c >= '0' && c <= '9');
    }

    bool isAlpha(char c)
    {
        return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_'));
    }

    bool isAlphaNumeric(char c)
    {
        return isDigit(c) || isAlpha(c);
    }

    bool isAtEnd()
    {
        return Current >= Source.Length;
    }
}
