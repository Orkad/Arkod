using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arkod.Sql
{
    public class SqlStuff
    {
        public void test()
        {
            var multiCommand = @"
-- this is an inline comment that should be ignored / ;
/*
this is a multiline comment that should be ignored / ;
*/
create table FOO (myid INT NOT NULL); -- this is an inline comment that should be ignored / ;
";
        }
        public string RemoveSqlComments(string script)
        {
            var buffer = new StringBuilder();
            char previous = ' ';
            bool command = false;
            using (var reader = new StringReader(script))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        char current = line[i];
                        buffer.Append(current);
                        if (current == '-' && previous == '-')
                        {
                            buffer.Remove(i, 1);
                            break;
                        }

                        // end of char
                        previous = current;
                    }

                    // end of current line
                }
            }
            return buffer.ToString();
        }
    }
}
