using Kamsyk.Reget.Model.Repositories.Interfaces;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using System.Collections;

namespace Kamsyk.Reget.Model.Repositories {
    public class DataGridRepository : BaseRepository<User_GridSetting> {
        #region Methods
        public User_GridSetting GetUserGridSettings(int userId, string gridId) {

            if (gridId.Trim().ToLower() == "grdusersubstitution_rg") {
                new SubstitutionRepository().SetDefaultFilter(userId);
            }
            
            var gridSettings = (from userGridDb in m_dbContext.User_GridSetting
                                where userGridDb.user_id == userId &&
                                userGridDb.grid_name == gridId
                                select userGridDb).FirstOrDefault();
           
            if (gridSettings == null) {
                return null;
            }

            

            User_GridSetting retGridSettings = new User_GridSetting();

            SetValues(gridSettings, retGridSettings);
            if (retGridSettings.sort == null) {
                retGridSettings.sort = "";
            }

            if (retGridSettings.filter == null) {
                retGridSettings.filter = "";
            }

            if (retGridSettings.columns == null) {
                retGridSettings.columns = "";
            }

            return retGridSettings;
        }

        public void SetUserGridSettings(User_GridSetting gridSettings) {
            int userId = gridSettings.user_id;
            string gridId = gridSettings.grid_name;

            var dbGridSettings = (from userGridDb in m_dbContext.User_GridSetting
                                where userGridDb.user_id == userId &&
                                userGridDb.grid_name == gridId
                                select userGridDb).FirstOrDefault();

            if (dbGridSettings == null) {
                User_GridSetting newgridSettings = new User_GridSetting();
                newgridSettings.user_id = userId;
                newgridSettings.grid_name = gridId;
                newgridSettings.grid_page_size = gridSettings.grid_page_size;
                newgridSettings.filter = gridSettings.filter;
                newgridSettings.sort = gridSettings.sort;
                newgridSettings.columns = gridSettings.columns;

                m_dbContext.User_GridSetting.Add(newgridSettings);
            } else {
                dbGridSettings.grid_page_size = gridSettings.grid_page_size;
                dbGridSettings.filter = gridSettings.filter;
                dbGridSettings.sort = gridSettings.sort;
                dbGridSettings.columns = gridSettings.columns;
            }

            m_dbContext.SaveChanges();
        }

        public void DeleteUserGridSettings(User_GridSetting gridSettings) {
            int userId = gridSettings.user_id;
            string gridId = gridSettings.grid_name;

            var dbGridSettings = (from userGridDb in m_dbContext.User_GridSetting
                                  where userGridDb.user_id == userId &&
                                  userGridDb.grid_name == gridId
                                  select userGridDb).FirstOrDefault();

            if (dbGridSettings != null) {
                m_dbContext.User_GridSetting.Remove(dbGridSettings);
                m_dbContext.SaveChanges();
            }

            
        }
        #endregion
    }
}
