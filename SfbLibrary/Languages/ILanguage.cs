// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary.Languages
{
    public interface ILanguage
    {
        #region Character of each class
        string Name { get; }
        string FilePath { get; }
        string Version { get; }
        string ApplicationVersion { get; }
        string Author { get; }
        string Company { get; }
        string AuthorUrl { get; }
        #endregion

        #region Settings
        string formSettings_btnSave { get; }
        string formSettings_btnCancel { get; }
        string formSettings_Text { get; }
        string formSettings_labelOptions { get; }
        #endregion

        #region VideoControlBar
        string videoControlBar_labelSubtilesOnOffText { get; }
        string videoControlBar_labelOn { get; }
        string videoControlBar_labelOff { get; }
        string videoControlBar_fullscreenOff { get; }
        string videoControlBar_fullscreenOn { get; }
        string videoControlBar_startTxt { get; }
        string videoControlBar_subtilesOn { get; }
        string videoControlBar_subtilesOff { get; }
        string videoControlBar_buttonPlay { get; }
        string videoControlBar_buttonPause { get; }
        string videoControlBar_buttonStop { get; }
        string videoControlBar_buttonRewind { get; }
        string videoControlBar_buttonForward { get; }
        string videoControlBar_playing { get; }
        string videoControlBar_paused { get; }
        string videoControlBar_stoped { get; }
        string videoControlBar_openingVideo { get; }
        string videoControlBar_openingSubtile { get; }
        string videoControlBar_volumeUp { get; }
        string videoControlBar_volumeDown { get; }
        string videoControlBar_volumeMute { get; }
        #endregion

        #region Event label
        string eventLabel_OpenVideoFile { get; }
        #endregion
    }
}
