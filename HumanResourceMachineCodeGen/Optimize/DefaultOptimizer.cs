using HumanResourceMachineCodeGen.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Optimize
{
    public class DefaultOptimizer
    {
        public static void ForceOptimize(List<HRMCode> codes, out List<HRMCode> opt)
        {
            List<HRMCode> tmp = new List<HRMCode>();
            while (Optimize(codes, out tmp))
                codes = tmp;
            opt = tmp;
        }

        public static bool Optimize(List<HRMCode> codes, out List<HRMCode> opt)
        {
            bool optimized = false;
            List<HRMCode> oij;
            optimized |= ImdJmp(codes, out oij);
            List<HRMCode> odc;
            optimized |= DeadCode(oij, out odc);
            List<HRMCode> onmc;
            optimized |= NoModCpy(odc, out onmc);
            List<HRMCode> oic;
            optimized |= ImdCpy(onmc, out oic);
            List<HRMCode> ourl;
            optimized |= UnrefLab(oic, out ourl);
            List<HRMCode> oej;
            optimized |= EmptyJmp(ourl, out oej);
            opt = oej;
            return optimized;
        }

        private static bool EmptyJmp(List<HRMCode> codes, out List<HRMCode> opt)
        {
            bool done = false;
            opt = new List<HRMCode>(codes);
            for (int i = opt.Count - 1; i > 0;)
            {
                if (opt[i].Instruction == HRMInstr.Lab &&
                    (opt[i - 1].Instruction == HRMInstr.Jmp ||
                    opt[i - 1].Instruction == HRMInstr.Jn ||
                    opt[i - 1].Instruction == HRMInstr.Jz) &&
                    opt[i].Param == opt[i - 1].Param)
                {
                    opt.RemoveRange(i - 1, 2);
                    i -= 2;
                    done = true;
                }
                else if (opt[i - 1].Instruction == HRMInstr.Lab &&
                    (opt[i].Instruction == HRMInstr.Jmp ||
                    opt[i].Instruction == HRMInstr.Jn ||
                    opt[i].Instruction == HRMInstr.Jz) &&
                    opt[i].Param == opt[i - 1].Param)
                {
                    opt.RemoveRange(i - 1, 2);
                    i -= 2;
                    done = true;
                }
                else
                    i--;
            }
            return done;
        }

        private static bool UnrefLab(List<HRMCode> codes, out List<HRMCode> opt)
        {
            bool done = false;
            opt = new List<HRMCode>(codes);
            HashSet<int> used = new HashSet<int>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Instruction == HRMInstr.Jmp ||
                    codes[i].Instruction == HRMInstr.Jn ||
                    codes[i].Instruction == HRMInstr.Jz)
                    used.Add(codes[i].Param);
            }
            for (int i = opt.Count - 1; i >= 0; i--)
                if (opt[i].Instruction == HRMInstr.Lab)
                    if (!used.Contains(opt[i].Param))
                    {
                        opt.RemoveAt(i);
                        done = true;
                    }
            return done;
        }

        private static bool ImdCpy(List<HRMCode> codes, out List<HRMCode> opt)
        {
            /*
             * inc/dec x / copyto x
             * - copyfrom x
             */
            bool done = false;
            opt = new List<HRMCode>();
            for (int i = 0; i < codes.Count; i++)
            {
                opt.Add(codes[i]);
                if (codes[i].Instruction == HRMInstr.Inc ||
                    codes[i].Instruction == HRMInstr.Dec ||
                    (i + 1 < codes.Count &&
                    codes[i].Instruction == HRMInstr.Cpyt &&
                    codes[i].Param == codes[i + 1].Param))
                {
                    if (i + 1 < codes.Count &&
                        codes[i + 1].Instruction == HRMInstr.Cpyf &&
                        codes[i].Param == codes[i + 1].Param)
                        i++;
                }
            }
            return done;
        }

        private static bool NoModCpy(List<HRMCode> codes, out List<HRMCode> opt)
        {
            /*
             * copyfrom x
             * (no modification)
             * (copyto)
             * (jmp,jn,jz)
             * - copyfrom x
             */
            bool done = false;
            opt = new List<HRMCode>(codes);
            for (int i = codes.Count - 1; i >= 0; )
            {
                if (opt[i].Instruction == HRMInstr.Cpyf)
                {
                    int j = i - 1;
                    while (j >= 0 &&
                        (opt[j].Instruction == HRMInstr.Cpyt ||
                        opt[j].Instruction == HRMInstr.Jmp ||
                        opt[j].Instruction == HRMInstr.Jz ||
                        opt[j].Instruction == HRMInstr.Jn))
                        j--;
                    if (j >= 0 && i >= 0 &&
                        opt[j].Instruction == HRMInstr.Cpyf &&
                        opt[j].Param == opt[i].Param)
                        opt.RemoveAt(i);
                    i = j;
                }
                else
                    i--;
            }
            return done;
        }

        private static bool DeadCode(List<HRMCode> codes, out List<HRMCode> opt)
        {
            /*
             * jmp ???
             * ***
             * (no label)
             * ***
             */
            bool done = false;
            opt = new List<HRMCode>();
            for (int i = 0; i < codes.Count; )
            {
                opt.Add(codes[i]);
                if (codes[i].Instruction != HRMInstr.Jmp)
                {
                    i++;
                }
                else
                {
                    int j = i + 1;
                    for (; j < codes.Count; j++)
                        if (codes[j].Instruction == HRMInstr.Lab)
                        {
                            opt.Add(codes[j]);
                            break;
                        }
                    i = j + 1;
                }
            }
            return done;
        }

        private static bool ImdJmp(List<HRMCode> codes, out List<HRMCode> opt)
        {
            /*
             * label
             * jmp ???
             */
            bool done = false;
            List<HRMCode> tmp = new List<HRMCode>(codes);
            for (int i = 0; i < codes.Count - 1;)
            {
                if (tmp[i].Instruction == HRMInstr.Lab &&
                    tmp[i + 1].Instruction == HRMInstr.Jmp)
                {
                    done = true;
                    int tar = tmp.FindIndex((HRMCode c) =>
                      {
                          return c.Instruction == HRMInstr.Lab && c.Param == tmp[i + 1].Param;
                      });
                    if (tar < i)
                    {
                        HRMCode c = tmp[i];
                        tmp.RemoveAt(i);
                        tmp.Insert(tar, c);
                        i += 2;
                    }
                    else
                    {
                        HRMCode c = tmp[i];
                        tmp.Insert(tar, c);
                        tmp.RemoveAt(i);
                        i++;
                    }
                }
                else
                    i++;
            }
            opt = tmp;
            return done;
        }
    }
}
