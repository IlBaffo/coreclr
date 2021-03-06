// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**************************************************************
/* a basic test case for GC with cyclic Double linked list leaks.
/* Creat a DoubLink object which is a cyclic Double linked list
/* object with iObj number node. then deletes its reference when
/* the next object is created. Do this loop iRep times.
/**************************************************************/

namespace DoubLink {
    using System;

    public class DoubLinkGen
    {
// disabling unused variable warning
#pragma warning disable 0414
        internal DoubLink Mv_Doub;
#pragma warning restore 0414

        public static int Main(System.String [] Args)
        {
            int iRep = 0;
            int iObj = 0;

            Console.WriteLine("Test should return with ExitCode 100 ...");
            switch( Args.Length )
            {
                case 1:
                    if (!Int32.TryParse( Args[0], out iRep ))
                    {
                        iRep = 100;
                    }
                break;

                case 2:
                    if (!Int32.TryParse( Args[0], out iRep ))
                    {
                        iRep = 100;
                    }
                    if (!Int32.TryParse( Args[1], out iObj ))
                    {
                        iObj = 10;
                    }
                break;

                default:
                    iRep = 100;
                    iObj = 10;
                break;
            }

            DoubLinkGen Mv_Leak = new DoubLinkGen();
            if(Mv_Leak.runTest(iRep, iObj ))
            {
                Console.WriteLine("Test Passed");
                return 100;
            }
            Console.WriteLine("Test Failed");
            return 1;
        }


        public bool runTest(int iRep, int iObj)
        {
            SetLink(iRep, iObj);
            Mv_Doub = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.Write(DLinkNode.FinalCount);
            Console.WriteLine(" DLinkNodes finalized");
            return (DLinkNode.FinalCount==iRep*iObj);

        }


        public void SetLink(int iRep, int iObj)
        {
            for(int i=0; i<iRep; i++)
            {
                Mv_Doub = new DoubLink(iObj);
            }

        }

    }

    public class DoubLink
    {
        internal DLinkNode[] Mv_DLink;

        public DoubLink(int Num)
            : this(Num, false)
        {
        }

        public DoubLink(int Num, bool large)
        {

            Mv_DLink = new DLinkNode[Num];

            if (Num == 0)
            {
                return;
            }

            if (Num == 1)
            {
                // only one element
                Mv_DLink[0] = new DLinkNode((large ? 250 : 1), Mv_DLink[0], Mv_DLink[0]);
                return;
            }

            // first element
            Mv_DLink[0] = new DLinkNode((large ? 250 : 1), Mv_DLink[Num - 1], Mv_DLink[1]);

            // all elements in between
            for (int i = 1; i < Num - 1; i++)
            {
                Mv_DLink[i] = new DLinkNode((large ? 250 : i + 1), Mv_DLink[i - 1], Mv_DLink[i + 1]);
            }

            // last element
            Mv_DLink[Num - 1] = new DLinkNode((large ? 250 : Num), Mv_DLink[Num - 2], Mv_DLink[0]);
        }


        public int NodeNum
        {
            get
            {
                return Mv_DLink.Length;
            }
        }


        public DLinkNode this[int index]
        {
            get
            {
                return Mv_DLink[index];
            }

            set
            {
                Mv_DLink[index] = value;
            }
        }

    }

    public class DLinkNode
    {
        // disabling unused variable warning
#pragma warning disable 0414
        internal DLinkNode Last;
        internal DLinkNode Next;
        internal int[] Size;
#pragma warning restore 0414

        public static int FinalCount = 0;

        public DLinkNode(int SizeNum, DLinkNode LastObject, DLinkNode NextObject)
        {
            Last = LastObject;
            Next = NextObject;
            Size = new int[SizeNum * 1024];
        }

        ~DLinkNode()
        {
            FinalCount++;
        }
    }
}
