using System;
using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.Interfaces.ImportExcelData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using System.Linq;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ApplicationLayer.ImportExcelData
{
    public class ImportbrethrentoDb : IImporttoDb
    {
        private readonly IBrethrenManager _brethrenManager;
        private readonly IGroupManager _groupManager;
        private readonly DateTime _lastStatusUpdate;
        private string _statusmessage = "No message";

        public ImportbrethrentoDb(IGroupManager groupManager, IBrethrenManager brethrenManager,
            DateTime lastStatusUpdate){
            _groupManager = groupManager;
            _brethrenManager = brethrenManager;
            _lastStatusUpdate = lastStatusUpdate;
        }

        private void AssignBrethrenToGroup(BrethrenBasic brethren, string groupName){

            var group = _groupManager.Getallgroup().FirstOrDefault(g => g.GroupName == groupName);
            if (group != null)
                _groupManager.AddBrethrenToAGroup(brethren.Id, group.Id);
        }

        private CivilStatus getcivilstatues(string strcivilstatues){
            CivilStatus civilStatus;
            switch (strcivilstatues){
                case "SINGLE":
                case "Single":
                    civilStatus = CivilStatus.Single;
                    break;
                case "MARRIED":
                case "Married":
                    civilStatus = CivilStatus.Married;
                    break;
                case "WIDOW":
                case "Widow":
                    civilStatus = CivilStatus.Widow;
                    break;
                case "SEPARATED":
                case "Separeted":
                    civilStatus = CivilStatus.Separated;
                    break;
                default:
                    civilStatus = CivilStatus.No_Info;
                    break;
            }
            return civilStatus;
        }

        private Gender getgender(string strgender){
            Gender gender;
            switch (strgender){
                case "M":
                case "Male":
                    gender = Gender.Male;
                    break;
                case "F":
                case "Female":
                    gender = Gender.Female;
                    break;
                default:
                    gender = Gender.Male;
                    break;
            }
            return gender;
        }

        private Nullable<DateTime> Parsedatetime(string strdate){
            Nullable<DateTime> nulldateTime = null;
            DateTime dateTime;
            if (DateTime.TryParse(strdate, out dateTime)){
                nulldateTime = dateTime;
            }

            return nulldateTime;
        }

        public bool Import(List<DatatoImport> members){
            try{
                var memberslist = new List<BrethrenBasic>();

                foreach (var datatoImport in members){
                    if (!string.IsNullOrWhiteSpace(datatoImport.Name))
                        memberslist.Add(new BrethrenBasic{
                            ChurchId = datatoImport.ChurchId,
                            Name = datatoImport.Name,
                            LocalStatus = LocalStatus.Present_Here,
                            LastStatusUpdate = _lastStatusUpdate,
                            BrethrenFull = new BrethrenFull{
                                Baptizer = datatoImport.Baptizer,
                                Barangay = datatoImport.Barangay,
                                Contactno = datatoImport.Contactno,
                                Gender = getgender(datatoImport.Gender),
                                DateofBaptism = Parsedatetime(datatoImport.DateofBaptism),
                                CivilStatus = getcivilstatues(datatoImport.CivilStatus),
                                Language = datatoImport.Language,
                                DateofBirth = Parsedatetime(datatoImport.DateofBirth),
                                EducationalAttainment = datatoImport.EducationalAttainment,
                                NickName = datatoImport.NickName,
                                Job = datatoImport.Job,
                                PlaceofBaptism = datatoImport.PlaceofBaptism,
                                Skills = datatoImport.Skills,
                                StreetNumber = datatoImport.StreetNumber,
                                Street = datatoImport.Street,
                                Town = datatoImport.Town,
                                Province = datatoImport.Province
                            }
                        });
                }

                foreach (var brethrenBasic in memberslist){
                    _brethrenManager.AddBrethren(brethrenBasic);
                }
                foreach (var brethrenBasic in memberslist){
                    if (!string.IsNullOrWhiteSpace(members.FirstOrDefault(b => b.Name == brethrenBasic.Name).Group))
                        AssignBrethrenToGroup(brethrenBasic, members.FirstOrDefault(b => b.Name == brethrenBasic.Name).Group);
                }
            }
            catch (Exception exception){
                _statusmessage = exception.Message;
                return false;
            }
            _statusmessage = "Success Importing brethren";
            return true;
        }


        public string Statusmessage(){
            return _statusmessage;
        }
    }
}