using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    /*
     * Interface TES5Expression
     * @package Ormin\OBSLexicalParser\TES5\AST\Expression
     *
     * Implementers declare that you can evaluate it as an expression and return a value.
     */
    interface ITES5Expression : ITES5Value, ITES5CodeChunk//WTM:  Change:  Added ITES5CodeChunk.
    {
    }
}