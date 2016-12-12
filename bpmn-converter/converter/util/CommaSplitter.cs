using System;
using System.Collections.Generic;
using bpmn_converter.converter.util;

namespace org.activiti.bpmn.converter.util
{

    public class CommaSplitter
    {
        // 如果它们不在表达式内，则使用逗号分割给定的表达式
        public static List<string> splitCommas(string st)
        {
            List<string> result = new List<string>();
            int offset = 0;

            bool inExpression = false;
            for (int i = 0; i < st.length(); i++)
            {
                if (inExpression == false && st[i] == ',')
                {
                    if ((i - offset) > 1)
                    {
                        result.Add(st.Substring(offset, i));
                    }
                    offset = i + 1;
                }
                else if ((st[i] == '$' || st[i] == '#') && st[i + 1] == '{')
                {
                    inExpression = true;
                }
                else if (st[i] == '}')
                {
                    inExpression = false;
                }
            }

            if ((st.length() - offset) > 1)
            {
                result.Add(st.Substring(offset));
            }
            return result;
        }
    }
}
