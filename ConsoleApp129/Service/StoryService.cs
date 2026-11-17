using ConsoleApp129.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp129.Services
{
    internal static class StoryService
    {
        public static void ShowIntro()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("           ХРОНИКИ ПЯТИ ПЕЧАТЕЙ");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Мир Аэтэрии расколот. Пять Древних Печатей,");
            Console.WriteLine("что сдерживали стихии, рассыпались на осколки.");
            Console.WriteLine();
            Console.WriteLine("Ты — последний Хранитель. Собери все пять печатей.");
            Console.WriteLine("Каждая — 7 осколков одного вида в форме «П».");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("!!! ВАЖНО !!!");
            Console.ResetColor();
            Console.WriteLine("Первая и последняя печати определят финал.");
            Console.WriteLine("20 возможных концовок. Выбор за тобой.");
            Console.WriteLine();
            Console.WriteLine("  ♦  Пламя  — Огонь и Хаос");
            Console.WriteLine("  ❄  Лёд    — Холод и Порядок");
            Console.WriteLine("  §  Гром   — Сила и Власть");
            Console.WriteLine("  ☾  Тьма   — Мрак и Тайна");
            Console.WriteLine("  ☼  Свет   — Надежда и Жизнь");
            Console.WriteLine();
            ConsoleHelper.Pause();
        }

        public static void ShowPortalEntryStory(int type, List<int> history)
        {
            Console.Clear();
            Console.ForegroundColor = ArtifactInfo.GetColor(type);
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("       ПОРТАЛ: " + ArtifactInfo.GetName(type).ToUpper());
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();

            if (history.Count == 0)
                Console.WriteLine("Это твой ПЕРВЫЙ портал. Стихия запомнит тебя навсегда.");
            else
            {
                Console.Write("Ты уже прошёл: ");
                foreach (int t in history)
                {
                    Console.ForegroundColor = ArtifactInfo.GetColor(t);
                    Console.Write(ArtifactInfo.GetSymbol(t) + " ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            switch (type)
            {
                case 1:
                    Console.WriteLine("\nПламя вырывается из портала, обжигая лицо.");
                    if (history.Count == 0) Console.WriteLine("Владыка Пламени: «Ты пришёл первым к огню. Мир запомнит.»");
                    else Console.WriteLine("Владыка Пламени: «Огонь ждал тебя, Хранитель.»");
                    break;
                case 2:
                    Console.WriteLine("\nМороз сковывает дыхание.");
                    if (history.Count == 0) Console.WriteLine("Ледяной Страж: «Порядок вечен. Ты понял это первым.»");
                    else Console.WriteLine("Ледяной Страж: «Лёд помнит каждый твой шаг.»");
                    break;
                case 3:
                    Console.WriteLine("\nРаскаты грома сотрясают землю.");
                    if (history.Count == 0) Console.WriteLine("Голос Зала: «Ты пришёл за силой первым. Дерзко.»");
                    else Console.WriteLine("Голос Зала: «Гром любит смелых, Хранитель.»");
                    break;
                case 4:
                    Console.WriteLine("\nТьма окутывает тебя. Шёпот мёртвых.");
                    if (history.Count == 0) Console.WriteLine("Бездна: «Ты выбрал меня первым. Мудро.»");
                    else Console.WriteLine("Бездна: «Тьма помнит тебя.»");
                    break;
                case 5:
                    Console.WriteLine("\nТёплый свет окутывает тебя.");
                    if (history.Count == 0) Console.WriteLine("Святилище: «Ты пришёл к свету первым. Это судьба.»");
                    else Console.WriteLine("Святилище: «Свет всегда ждёт тебя.»");
                    break;
            }
        }

        public static void ShowPortalCompleteStory(int type, List<int> history)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("   ПЕЧАТЬ " + ArtifactInfo.GetName(type).ToUpper() + " ОСВОБОЖДЕНА");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Сила «" + ArtifactInfo.GetName(type) + "» вливается в тебя.");
            Console.WriteLine();

            int count = history.Count;
            if (count == 1) Console.WriteLine("Одна из пяти сил. Путь начат.");
            else if (count == 2) Console.WriteLine("Две силы. Они борются или сливаются?");
            else if (count == 3) Console.WriteLine("Три печати. Половина пути пройдена.");
            else if (count == 4) Console.WriteLine("Четыре печати. Осталась последняя...");
            Console.WriteLine("\nТы возвращаешься в мир...");
        }

        public static void ShowEnding(List<int> history)
        {
            if (history.Count < 5)
            {
                ShowIncompleteEnding(history);
                return;
            }

            int first = history[0];
            int last = history[4];

            Console.Clear();
            Console.Write("Твой путь: ");
            foreach (int t in history)
            {
                Console.ForegroundColor = ArtifactInfo.GetColor(t);
                Console.Write(ArtifactInfo.GetSymbol(t) + " ");
            }
            Console.ResetColor();
            Console.WriteLine("\n");
            ConsoleHelper.Pause();

            string title = GetEndingTitle(first, last);
            string body = GetEndingBody(first, last);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("  КОНЦОВКА: " + title);
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(body);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("           СПАСИБО ЗА ИГРУ!");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.ResetColor();
        }

        private static string GetEndingTitle(int first, int last)
        {
            switch (first * 10 + last)
            {
                case 12: return "«ПЕПЕЛ ПОД СНЕГОМ»";
                case 13: return "«ВОССТАНИЕ ГРОЗЫ»";
                case 14: return "«СГОРЕВШАЯ ДУША»";
                case 15: return "«ФЕНИКС ВОССТАЛ»";
                case 21: return "«РАСТАЯВШИЙ ТРОН»";
                case 23: return "«ЛЕДЯНЫЕ МОЛНИИ»";
                case 24: return "«ВЕЧНАЯ МЕРЗЛОТА»";
                case 25: return "«КРИСТАЛЬНАЯ ЗАРЯ»";
                case 31: return "«БУРЯ В ОГНЕ»";
                case 32: return "«ОКАМЕНЕВШАЯ МОЛНИЯ»";
                case 34: return "«ТИХИЙ РАСКАТ»";
                case 35: return "«ГОЛОС НЕБЕС»";
                case 41: return "«ПРОБУЖДЕНИЕ БЕЗДНЫ»";
                case 42: return "«ХОЛОДНАЯ ТИШИНА»";
                case 43: return "«ТЁМНАЯ БУРЯ»";
                case 45: return "«РАССВЕТ ПОСЛЕ НОЧИ»";
                case 51: return "«ЗАКАТ НАДЕЖДЫ»";
                case 52: return "«ЗАМОРОЖЕННАЯ ЗАРЯ»";
                case 53: return "«ОСЛЕПЛЯЮЩАЯ МОЛНИЯ»";
                case 54: return "«ПОГАСШЕЕ СОЛНЦЕ»";
                default: return "«НЕИЗВЕСТНАЯ СУДЬБА»";
            }
        }

        private static string GetEndingBody(int first, int last)
        {
            switch (first * 10 + last)
            {
                case 12:
                    return
                        "Ты начал с Огня и закончил Льдом.\n" +
                        "Страсть сменилась вечным покоем. Мир обрёл тишину,\n" +
                        "но потерял тепло. Ты стал легендой о том,\n" +
                        "как буря может утихнуть — навсегда.\n\n" +
                        "Мир будет холодным, но стабильным. И это твой выбор.";
                case 13:
                    return
                        "Ты начал с Огня и закончил Громом.\n" +
                        "Страсть обратилась в силу. Мир склонился\n" +
                        "перед бурей. Ты стал завоевателем,\n" +
                        "чьё имя грохочет в веках.\n\n" +
                        "Твой огонь превратился в молнию. Никто не смеет\n" +
                        "смотреть тебе в глаза.";
                case 14:
                    return
                        "Ты начал с Огня и закончил Тьмой.\n" +
                        "Страсть угасла, став тайной. Мир погрузился\n" +
                        "в тени. Ты — тот, кто сгорел изнутри,\n" +
                        "но снаружи стал холодным и мудрым.\n\n" +
                        "Тени шепчут твоё имя. Живые боятся.";
                case 15:
                    return
                        "Ты начал с Огня и закончил Светом.\n" +
                        "Разрушение привело к возрождению. Мир исцелён.\n" +
                        "Ты — Феникс, восставший из пепла.\n\n" +
                        "Твоя страсть очистилась, стала надеждой.\n" +
                        "Аэтэрия помнит тебя как спасителя.";
                case 21:
                    return
                        "Ты начал со Льда и закончил Огнём.\n" +
                        "Вечный порядок рухнул в революции.\n" +
                        "Трон растаял. Ты — освободитель или разрушитель?\n\n" +
                        "Мир не знает. Но он изменился навсегда.\n" +
                        "Из пепла старого восстанет новое.";
                case 23:
                    return
                        "Ты начал со Льда и закончил Громом.\n" +
                        "Порядок обратился в силу. Молнии бьют\n" +
                        "в вечной мерзлоте. Ты — воплощение\n" +
                        "дисциплины и мощи.\n\n" +
                        "Мир застыл, но каждое движение — гром.";
                case 24:
                    return
                        "Ты начал со Льда и закончил Тьмой.\n" +
                        "Порядок стал тайной. Мир замёрз во мраке.\n" +
                        "Ты — хранитель вечной тишины,\n" +
                        "что не нарушит ни звук, ни свет.\n\n" +
                        "Никто не знает, что ты здесь. И это правильно.";
                case 25:
                    return
                        "Ты начал со Льда и закончил Светом.\n" +
                        "Порядок стал надеждой. Мир засиял\n" +
                        "кристальной чистотой. Ты — провозвестник\n" +
                        "новой эры.\n\n" +
                        "Аэтэрия расцвела. Радуга прошла по небу.";
                case 31:
                    return
                        "Ты начал с Грома и закончил Огнём.\n" +
                        "Сила обратилась в революцию. Небо горит.\n" +
                        "Ты — гроза, ставшая пожаром.\n\n" +
                        "Мир изменился. Старое сгорело в твоём пламени.";
                case 32:
                    return
                        "Ты начал с Грома и закончил Льдом.\n" +
                        "Сила застыла в порядке. Молния вмёрзла в лёд.\n" +
                        "Ты — вечный памятник победе.\n\n" +
                        "Мир помнит твой гром — но слышит тишину.";
                case 34:
                    return
                        "Ты начал с Грома и закончил Тьмой.\n" +
                        "Сила стала тайной. Мир слышит только шёпот.\n" +
                        "Ты — та власть, что не кричит, но правит.\n\n" +
                        "Тени подчиняются тебе. Никто не знает,\n" +
                        "откуда исходит приказ.";
                case 35:
                    return
                        "Ты начал с Грома и закончил Светом.\n" +
                        "Сила стала надеждой. Небеса поют.\n" +
                        "Ты — голос, что несёт благую весть.\n\n" +
                        "Твой гром стал музыкой. Мир слушает и радуется.";
                case 41:
                    return
                        "Ты начал с Тьмы и закончил Огнём.\n" +
                        "Тайна вспыхнула страстью. Бездна пробудилась.\n" +
                        "Ты — хаос, ставший пламенем.\n\n" +
                        "Мир не знал, что ждать. И это его спасло.";
                case 42:
                    return
                        "Ты начал с Тьмы и закончил Льдом.\n" +
                        "Тайна стала порядком. Мир застыл\n" +
                        "в вечной ночи. Ты — тайна, обращённая в лёд.\n\n" +
                        "Никто не нарушит твой покой. Никто не потревожит.";
                case 43:
                    return
                        "Ты начал с Тьмы и закончил Громом.\n" +
                        "Тайна стала силой. Тьма грохочет.\n" +
                        "Ты — бездна, что обрушивается бурей.\n\n" +
                        "Мир слышит твой шаг как раскат. И дрожит.";
                case 45:
                    return
                        "Ты начал с Тьмы и закончил Светом.\n" +
                        "Тайна стала надеждой. Ночь сменилась зарёй.\n" +
                        "Ты — тот, кто прошёл через мрак и обрёл свет.\n\n" +
                        "Величайшее искупление. Мир помнит твой путь\n" +
                        "и рассказывает его как легенду.";
                case 51:
                    return
                        "Ты начал со Света и закончил Огнём.\n" +
                        "Надежда обернулась разрушением. Свет стал пожаром.\n" +
                        "Ты — падший светоносец.\n\n" +
                        "Трагедия: тот, кто должен был спасти мир,\n" +
                        "сжёг его. Но из пепла восстанет новое.";
                case 52:
                    return
                        "Ты начал со Света и закончил Льдом.\n" +
                        "Надежда застыла в вечности. Заря больше\n" +
                        "не наступит. Ты — свет, что замёрз.\n\n" +
                        "Мир ждал тепла — получил холод.\n" +
                        "Но холод этот — чистый и вечный.";
                case 53:
                    return
                        "Ты начал со Света и закончил Громом.\n" +
                        "Надежда стала силой. Свет бьёт молнией.\n" +
                        "Ты — тот, кто правит небесами.\n\n" +
                        "Ослепительная красота. И ослепительная мощь.";
                case 54:
                    return
                        "Ты начал со Света и закончил Тьмой.\n" +
                        "Надежда обернулась отчаянием. Солнце погасло.\n" +
                        "Ты — трагическая легенда.\n\n" +
                        "Мир помнит, как ты принёс свет — и как потерял его.\n" +
                        "Это самая печальная из историй.";
                default:
                    return "Твоя судьба осталась нерешённой.";
            }
        }

        private static void ShowIncompleteEnding(List<int> history)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("          НЕЗАВЕРШЁННЫЙ ПУТЬ");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();

            if (history.Count == 0)
            {
                Console.WriteLine("Ты не открыл ни одного портала.");
                Console.WriteLine("Мир остался во тьме, а легенда о тебе не родилась.");
            }
            else
            {
                Console.Write("Ты успел открыть: ");
                foreach (int t in history)
                {
                    Console.ForegroundColor = ArtifactInfo.GetColor(t);
                    Console.Write(ArtifactInfo.GetSymbol(t) + " ");
                }
                Console.ResetColor();
                Console.WriteLine("(" + string.Join(" → ", history) + ")");
                Console.WriteLine();
                Console.WriteLine("Ты прошёл только " + history.Count + " из 5 печатей.");
                Console.WriteLine("Судьба мира осталась нерешённой...");
                Console.WriteLine();
                Console.WriteLine("Первый портал: " + ArtifactInfo.GetName(history[0]));
                Console.WriteLine("Последний: " + ArtifactInfo.GetName(history[history.Count - 1]));
            }
            Console.WriteLine();
        }

        public static void ShowDeathNarrative(List<int> history)
        {
            if (history.Count == 0)
                Console.WriteLine("Ты погиб, не начав путь. Мир остался во тьме.");
            else if (history.Count < 3)
                Console.WriteLine("Ты пал в начале пути. Только " + history.Count +
                    " печать(и) обретены. Легенда не дописана.");
            else if (history.Count < 5)
                Console.WriteLine("Ты был так близок... " + history.Count + " из 5.");
            else
                Console.WriteLine("Ты прошёл весь путь, но пал в самом конце.");
        }
    }
}