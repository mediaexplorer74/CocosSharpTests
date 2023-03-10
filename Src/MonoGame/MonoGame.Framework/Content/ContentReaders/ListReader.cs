#region License
/*
MIT License
Copyright ? 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License


using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    internal class ListReader<T> : ContentTypeReader<List<T>>
    {
        ContentTypeReader elementReader;

        public ListReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
			Type readerType = typeof(T);
			elementReader = manager.GetTypeReader(readerType);
        }

        public override bool CanDeserializeIntoExistingObject
        {
            get { return true; }
        }

        protected internal override List<T> Read(ContentReader input, List<T> existingInstance)
        {
            int count = input.ReadInt32();
            List<T> list = existingInstance;
            if (list == null) list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                if (ReflectionHelpers.IsValueType(typeof(T)))
				{
                	list.Add(input.ReadObject<T>(elementReader));
				}
				else
				{
                    var readerType = input.Read7BitEncodedInt();
                	list.Add(readerType > 0 ? input.ReadObject<T>(input.TypeReaders[readerType - 1]) : default(T));
				}
            }
            return list;
        }
    }
}
