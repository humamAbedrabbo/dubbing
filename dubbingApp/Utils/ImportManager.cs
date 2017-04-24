using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Excel;
using dubbingModel;
using System.Data.Entity;

namespace dubbingApp.Utils
{
    public class ImportManager
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        public void ImportFile(long orderTrnHdrIntno, string fileName)
        {
            using (var reader = new CsvReader(new ExcelParser(fileName)))
            {
                reader.Configuration.RegisterClassMap<MyClassMap>();
                var items = reader.GetRecords<CsvSubtitle>().ToList();
                var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);

                // delete related data
                
                var scenesDel = order.scenes;
                ctx.scenes.RemoveRange(scenesDel);
                ctx.SaveChanges();
                var sheetHdrsDel = order.dubbingSheetHdrs;
                ctx.dubbingSheetHdrs.RemoveRange(sheetHdrsDel);
                ctx.SaveChanges();

                // define dictionaries
                var sheetDic = new Dictionary<string, long>();
                var sceneDic = new Dictionary<short, long>();
                var dlgDic = new Dictionary<KeyValuePair<short, short>, long>();
                var characters = order.agreementWork.workCharacters.ToDictionary(x => x.characterName.ToUpper(), y => y);

                // import sheetHdrs
                var list = items.OrderBy(x => x.Scene).ThenBy(x => x.Dialog);
                short sno = 1;
                foreach(var item in list)
                {
                    // 1- Import dubbSheetHdr
                    if (sheetDic.ContainsKey(item.Character.ToUpper()))
                        item.SheetHdrId = sheetDic[item.Character.ToUpper()];
                    else
                    {
                        // Add sheetHdr
                        var hdr = ctx.dubbingSheetHdrs.Create();
                        hdr.characterName = item.Character;
                        hdr.orderTrnHdrIntno = orderTrnHdrIntno;
                        hdr.voiceActorIntno = 0;
                        hdr.actorName = "ANONYMOUS";
                        if(characters.ContainsKey(item.Character.ToUpper()))
                        {
                            var chr = characters[item.Character.ToUpper()];
                            hdr.workCharacterIntno = chr.workCharacterIntno;
                            var workActor = chr.workActors.FirstOrDefault(x => x.status == true);
                            if (workActor != null)
                            {
                                hdr.voiceActorIntno = workActor.voiceActorIntno;
                                hdr.actorName = workActor.voiceActor.fullName;
                            }
                        }

                        ctx.dubbingSheetHdrs.Add(hdr);
                        ctx.SaveChanges();
                        item.SheetHdrId = hdr.dubbSheetHdrIntno;
                        sheetDic[item.Character.ToUpper()] = hdr.dubbSheetHdrIntno;
                    }

                    // 2- Import Scene
                    if (sceneDic.ContainsKey(item.Scene))
                        item.SceneId = sceneDic[item.Scene];
                    else
                    {
                        // import scene
                        var newscene = ctx.scenes.Create();
                        newscene.orderTrnHdrIntno = orderTrnHdrIntno;
                        newscene.sceneNo = item.Scene;
                        newscene.isTaken = false;
                        newscene.startTimeCode = "00:00:00";
                        newscene.endTimeCode = "00:00:00";
                        newscene.startSecond = 0;
                        newscene.endSecond = 0;
                        ctx.scenes.Add(newscene);
                        ctx.SaveChanges();
                        sceneDic[item.Scene] = newscene.sceneIntno;
                        item.SceneId = newscene.sceneIntno;
                    }

                    // 3- Import Dialog
                    var k = new KeyValuePair<short, short>(item.Scene, item.Dialog);
                    if (dlgDic.ContainsKey(k))
                        item.DialogId = dlgDic[k];
                    else
                    {
                        // Add new dialog
                        var dlg = ctx.dialogs.Create();
                        dlg.sceneIntno = item.SceneId;
                        dlg.dialogNo = item.Dialog;
                        dlg.isTaken = false;
                        dlg.startTimeCode = "00:00:00";
                        dlg.endTimeCode = "00:00:00:00";
                        dlg.startSecond = 0;
                        dlg.endSecond = 0;
                        ctx.dialogs.Add(dlg);
                        ctx.SaveChanges();
                        item.DialogId = dlg.dialogIntno;
                        dlgDic[k] = dlg.dialogIntno;
                    }

                    var subtitle = ctx.subtitles.Create();
                    string[] startWithM = item.Start.Split(',');
                    string startCode = startWithM[0];
                    long startMilli = 0;
                    if(startWithM.Length>1)
                    {
                        long.TryParse(startWithM[1], out startMilli);
                    }

                    string[] endWithM = item.End.Split(',');
                    string endCode = endWithM[0];
                    long endMilli = 0;
                    if (endWithM.Length > 1)
                    {
                        long.TryParse(endWithM[1], out endMilli);
                    }

                    subtitle.dubbSheetHdrIntno = item.SheetHdrId;
                    subtitle.dialogIntno = item.DialogId;
                    subtitle.scentence = item.Subtitle;
                    subtitle.subtitleNo = sno;
                    subtitle.startTimeCode = startCode;
                    subtitle.endTimeCode = endCode;
                    subtitle.startSecond = TimeConverter.StringToSeconds(startCode);
                    subtitle.endSecond = TimeConverter.StringToSeconds(endCode);
                    subtitle.startMillisecond = startMilli;
                    subtitle.endMillisecond = endMilli;
                    ctx.subtitles.Add(subtitle);
                    ctx.SaveChanges();

                    sno += 1;
                }

            }
        }

    }

    public class CsvSubtitle
    {
        public int No { get; set; }
        public short Scene { get; set; }
        public short Dialog { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Character { get; set; }
        public string Subtitle { get; set; }
        public long SceneId { get; set; }
        public long DialogId { get; set; }
        public long SheetHdrId { get; set; }
    }
    public sealed class MyClassMap : CsvClassMap<CsvSubtitle>
    {
        public MyClassMap()
        {
            Map(m => m.No).Name("No");
            Map(m => m.Scene).Name("Scene");
            Map(m => m.Dialog).Name("Dialog");
            Map(m => m.Start).Name("Start");
            Map(m => m.End).Name("End");
            Map(m => m.Character).Name("Character");
            Map(m => m.Subtitle).Name("Subtitle");
        }
    }
}