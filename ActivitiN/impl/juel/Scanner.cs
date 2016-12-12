using System;
using System.Collections.Generic;
using System.Text;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */ 
namespace org.activiti.engine.impl.juel
{



	/// <summary>
	/// Handcrafted scanner.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class Scanner
	{
		/// <summary>
		/// Scan exception type
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public static class ScanException extends Exception
		public class ScanException : Exception
		{
			internal readonly int position;
			internal readonly string encountered;
			internal readonly string expected;
			public ScanException(int position, string encountered, string expected) : base(LocalMessages.get("error.scan", position, encountered, expected))
			{
				this.position = position;
				this.encountered = encountered;
				this.expected = expected;
			}
		}

		public class Token
		{
			internal readonly Symbol symbol;
			internal readonly string image;
			internal readonly int length;
			public Token(Symbol symbol, string image) : this(symbol, image, image.Length)
			{
			}
			public Token(Symbol symbol, string image, int length)
			{
				this.symbol = symbol;
				this.image = image;
				this.length = length;
			}
			public virtual Symbol Symbol
			{
				get
				{
					return symbol;
				}
			}
			public virtual string Image
			{
				get
				{
					return image;
				}
			}
			public virtual int Size
			{
				get
				{
					return length;
				}
			}
			public override string ToString()
			{
				return symbol.ToString();
			}
		}

		public class ExtensionToken : Token
		{
			public ExtensionToken(string image) : base(Scanner.Symbol.EXTENSION, image)
			{
			}
		}

		/// <summary>
		/// Symbol type
		/// </summary>
		public enum Symbol
		{
			EOF,
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			PLUS("'+'"), MINUS("'-'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			MUL("'*'"), DIV("'/'|'div'"), MOD("'%'|'mod'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			LPAREN("'('"), RPAREN("')'"),
			IDENTIFIER,
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			NOT("'!'|'not'"), AND("'&&'|'and'"), OR("'||'|'or'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			EMPTY("'empty'"), INSTANCEOF("'instanceof'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			INTEGER, FLOAT, TRUE("'true'"), FALSE("'false'"), STRING, NULL("'null'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			LE("'<='|'le'"), LT("'<'|'lt'"), GE("'>='|'ge'"), GT("'>'|'gt'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			EQ("'=='|'eq'"), NE("'!='|'ne'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			QUESTION("'?'"), COLON("':'"),
			TEXT,
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			DOT("'.'"), LBRACK("'['"), RBRACK("']'"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			COMMA("','"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
			START_EVAL_DEFERRED("'#{'"), START_EVAL_DYNAMIC("'${'"), END_EVAL("'}'"),
			EXTENSION // used in syntax extensions
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//			private final String @string;
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//			private Symbol()
	//		{
	//			this(null);
	//		}
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//			private Symbol(String @string)
	//		{
	//			this.@string = @string;
	//		}
		}
	public static partial class EnumExtensionMethods
	{
			public override static string ToString(this Symbol instance)
			{
				return @string == null ? "<" + name() + ">" : @string;
			}
	}

		private static readonly Dictionary<string, Token> KEYMAP = new Dictionary<string, Token>();
		private static readonly Dictionary<Symbol, Token> FIXMAP = new Dictionary<Symbol, Token>();

		private static void addFixToken(Token token)
		{
			FIXMAP[token.Symbol] = token;
		}

		private static void addKeyToken(Token token)
		{
			KEYMAP[token.Image] = token;
		}

		static Scanner()
		{
			addFixToken(new Token(Symbol.PLUS, "+"));
			addFixToken(new Token(Symbol.MINUS, "-"));
			addFixToken(new Token(Symbol.MUL, "*"));
			addFixToken(new Token(Symbol.DIV, "/"));
			addFixToken(new Token(Symbol.MOD, "%"));
			addFixToken(new Token(Symbol.LPAREN, "("));
			addFixToken(new Token(Symbol.RPAREN, ")"));
			addFixToken(new Token(Symbol.NOT, "!"));
			addFixToken(new Token(Symbol.AND, "&&"));
			addFixToken(new Token(Symbol.OR, "||"));
			addFixToken(new Token(Symbol.EQ, "=="));
			addFixToken(new Token(Symbol.NE, "!="));
			addFixToken(new Token(Symbol.LT, "<"));
			addFixToken(new Token(Symbol.LE, "<="));
			addFixToken(new Token(Symbol.GT, ">"));
			addFixToken(new Token(Symbol.GE, ">="));
			addFixToken(new Token(Symbol.QUESTION, "?"));
			addFixToken(new Token(Symbol.COLON, ":"));
			addFixToken(new Token(Symbol.COMMA, ","));
			addFixToken(new Token(Symbol.DOT, "."));
			addFixToken(new Token(Symbol.LBRACK, "["));
			addFixToken(new Token(Symbol.RBRACK, "]"));
			addFixToken(new Token(Symbol.START_EVAL_DEFERRED, "#{"));
			addFixToken(new Token(Symbol.START_EVAL_DYNAMIC, "${"));
			addFixToken(new Token(Symbol.END_EVAL, "}"));
			addFixToken(new Token(Symbol.EOF, null, 0));

			addKeyToken(new Token(Symbol.NULL, "null"));
			addKeyToken(new Token(Symbol.TRUE, "true"));
			addKeyToken(new Token(Symbol.FALSE, "false"));
			addKeyToken(new Token(Symbol.EMPTY, "empty"));
			addKeyToken(new Token(Symbol.DIV, "div"));
			addKeyToken(new Token(Symbol.MOD, "mod"));
			addKeyToken(new Token(Symbol.NOT, "not"));
			addKeyToken(new Token(Symbol.AND, "and"));
			addKeyToken(new Token(Symbol.OR, "or"));
			addKeyToken(new Token(Symbol.LE, "le"));
			addKeyToken(new Token(Symbol.LT, "lt"));
			addKeyToken(new Token(Symbol.EQ, "eq"));
			addKeyToken(new Token(Symbol.NE, "ne"));
			addKeyToken(new Token(Symbol.GE, "ge"));
			addKeyToken(new Token(Symbol.GT, "gt"));
			addKeyToken(new Token(Symbol.INSTANCEOF, "instanceof"));
		}

		private Token token_Renamed; // current token
		 private int position; // start position of current token
		private readonly string input;

		protected internal readonly StringBuilder builder = new StringBuilder();

		/// <summary>
		/// Constructor. </summary>
		/// <param name="input"> expression string </param>
		protected internal Scanner(string input)
		{
			this.input = input;
		}

		public virtual string Input
		{
			get
			{
				return input;
			}
		}

		/// <returns> current token </returns>
		public virtual Token Token
		{
			get
			{
				return token_Renamed;
			}
		}

		/// <returns> current input position </returns>
		public virtual int Position
		{
			get
			{
				return position;
			}
		}

		/// <returns> <code>true</code> iff the specified character is a digit </returns>
		protected internal virtual bool isDigit(char c)
		{
			return c >= '0' && c <= '9';
		}

		/// <param name="s"> name </param>
		/// <returns> token for the given keyword or <code>null</code> </returns>
		protected internal virtual Token keyword(string s)
		{
			return KEYMAP[s];
		}

		/// <param name="symbol"> </param>
		/// <returns> token for the given symbol </returns>
		protected internal virtual Token @fixed(Symbol symbol)
		{
			return FIXMAP[symbol];
		}

		protected internal virtual Token token(Symbol symbol, string value, int length)
		{
			return new Token(symbol, value, length);
		}

		protected internal virtual bool Eval
		{
			get
			{
				return token_Renamed != null && token_Renamed.Symbol != Symbol.TEXT && token_Renamed.Symbol != Symbol.END_EVAL;
			}
		}

		/// <summary>
		/// text token
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Token nextText() throws ScanException
		protected internal virtual Token nextText()
		{
			builder.Length = 0;
			int i = position;
			int l = input.Length;
			bool escaped = false;
			while (i < l)
			{
				char c = input[i];
				switch (c)
				{
					case '\\':
						if (escaped)
						{
							builder.Append('\\');
						}
						else
						{
							escaped = true;
						}
						break;
					case '#':
					case '$':
						if (i + 1 < l && input[i + 1] == '{')
						{
							if (escaped)
							{
								builder.Append(c);
							}
							else
							{
								return token(Symbol.TEXT, builder.ToString(), i - position);
							}
						}
						else
						{
							if (escaped)
							{
								builder.Append('\\');
							}
							builder.Append(c);
						}
						escaped = false;
						break;
					default:
						if (escaped)
						{
							builder.Append('\\');
						}
						builder.Append(c);
						escaped = false;
					break;
				}
				i++;
			}
			if (escaped)
			{
				builder.Append('\\');
			}
			return token(Symbol.TEXT, builder.ToString(), i - position);
		}

		/// <summary>
		/// string token
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Token nextString() throws ScanException
		protected internal virtual Token nextString()
		{
			builder.Length = 0;
			char quote = input[position];
			int i = position + 1;
			int l = input.Length;
			while (i < l)
			{
				char c = input[i++];
				if (c == '\\')
				{
					if (i == l)
					{
						throw new ScanException(position, "unterminated string", quote + " or \\");
					}
					else
					{
						c = input[i++];
						if (c == '\\' || c == quote)
						{
							builder.Append(c);
						}
						else
						{
							throw new ScanException(position, "invalid escape sequence \\" + c, "\\" + quote + " or \\\\");
						}
					}
				}
				else if (c == quote)
				{
					return token(Symbol.STRING, builder.ToString(), i - position);
				}
				else
				{
					builder.Append(c);
				}
			}
			throw new ScanException(position, "unterminated string", Convert.ToString(quote));
		}

		/// <summary>
		/// number token
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Token nextNumber() throws ScanException
		protected internal virtual Token nextNumber()
		{
			int i = position;
			int l = input.Length;
			while (i < l && isDigit(input[i]))
			{
				i++;
			}
			Symbol symbol = Symbol.INTEGER;
			if (i < l && input[i] == '.')
			{
				i++;
				while (i < l && isDigit(input[i]))
				{
					i++;
				}
				symbol = Symbol.FLOAT;
			}
			if (i < l && (input[i] == 'e' || input[i] == 'E'))
			{
				int e = i;
				i++;
				if (i < l && (input[i] == '+' || input[i] == '-'))
				{
					i++;
				}
				if (i < l && isDigit(input[i]))
				{
					i++;
					while (i < l && isDigit(input[i]))
					{
						i++;
					}
					symbol = Symbol.FLOAT;
				}
				else
				{
					i = e;
				}
			}
			return token(symbol, input.Substring(position, i - position), i - position);
		}

		/// <summary>
		/// token inside an eval expression
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Token nextEval() throws ScanException
		protected internal virtual Token nextEval()
		{
			char c1 = input[position];
			char c2 = position < input.Length - 1 ? input[position + 1] : (char)0;

			switch (c1)
			{
				case '*':
					return @fixed(Symbol.MUL);
				case '/':
					return @fixed(Symbol.DIV);
				case '%':
					return @fixed(Symbol.MOD);
				case '+':
					return @fixed(Symbol.PLUS);
				case '-':
					return @fixed(Symbol.MINUS);
				case '?':
					return @fixed(Symbol.QUESTION);
				case ':':
					return @fixed(Symbol.COLON);
				case '[':
					return @fixed(Symbol.LBRACK);
				case ']':
					return @fixed(Symbol.RBRACK);
				case '(':
					return @fixed(Symbol.LPAREN);
				case ')':
					return @fixed(Symbol.RPAREN);
				case ',':
					return @fixed(Symbol.COMMA);
				case '.':
					if (!isDigit(c2))
					{
						return @fixed(Symbol.DOT);
					}
					break;
				case '=':
					if (c2 == '=')
					{
						return @fixed(Symbol.EQ);
					}
					break;
				case '&':
					if (c2 == '&')
					{
						return @fixed(Symbol.AND);
					}
					break;
				case '|':
					if (c2 == '|')
					{
						return @fixed(Symbol.OR);
					}
					break;
				case '!':
					if (c2 == '=')
					{
						return @fixed(Symbol.NE);
					}
					return @fixed(Symbol.NOT);
				case '<':
					if (c2 == '=')
					{
						return @fixed(Symbol.LE);
					}
					return @fixed(Symbol.LT);
				case '>':
					if (c2 == '=')
					{
						return @fixed(Symbol.GE);
					}
					return @fixed(Symbol.GT);
				case '"':
				case '\'':
					return nextString();
			}

			if (isDigit(c1) || c1 == '.')
			{
				return nextNumber();
			}

			if (char.isJavaIdentifierStart(c1))
			{
				int i = position + 1;
				int l = input.Length;
				while (i < l && char.isJavaIdentifierPart(input[i]))
				{
					i++;
				}
				string name = input.Substring(position, i - position);
				Token keyword = keyword(name);
				return keyword == null ? token(Symbol.IDENTIFIER, name, i - position) : keyword;
			}

			throw new ScanException(position, "invalid character '" + c1 + "'", "expression token");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Token nextToken() throws ScanException
		protected internal virtual Token nextToken()
		{
			if (Eval)
			{
				if (input[position] == '}')
				{
					return @fixed(Symbol.END_EVAL);
				}
				return nextEval();
			}
			else
			{
				if (position + 1 < input.Length && input[position + 1] == '{')
				{
					switch (input[position])
					{
						case '#':
							return @fixed(Symbol.START_EVAL_DEFERRED);
						case '$':
							return @fixed(Symbol.START_EVAL_DYNAMIC);
					}
				}
				return nextText();
			}
		}

		/// <summary>
		/// Scan next token.
		/// After calling this method, <seealso cref="#getToken()"/> and <seealso cref="#getPosition()"/>
		/// can be used to retreive the token's image and input position. </summary>
		/// <returns> scanned token </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Token next() throws ScanException
		public virtual Token next()
		{
			if (token_Renamed != null)
			{
				position += token_Renamed.Size;
			}

			int length = input.Length;

			if (Eval)
			{
				while (position < length && char.IsWhiteSpace(input[position]))
				{
					position++;
				}
			}

			if (position == length)
			{
				return token_Renamed = @fixed(Symbol.EOF);
			}

			return token_Renamed = nextToken();
		}
	}

}