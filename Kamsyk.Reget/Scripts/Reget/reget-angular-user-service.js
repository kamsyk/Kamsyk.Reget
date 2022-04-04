var app = angular.module('RegetAppUserService', []);

app.service('regetuserservice', function () {
   
    this.filterParticipants = function (Participants, name) {

        var searchParticipants = [];

        //if (name === null || name.length === 0) {
        if(IsStringValueNullOrEmpty(name)) {
            return Participants;
        }

        angular.forEach(Participants, function (participant) {
            var partWoDia = name.trim().toLowerCase();//removeDiacritics(name).replace(' ', '').trim().toLowerCase();
            if (!IsStringValueNullOrEmpty(participant.surname) && participant.surname.toLowerCase().indexOf(name.toLowerCase()) > -1 ||
                !IsStringValueNullOrEmpty(participant.first_name) && participant.first_name.toLowerCase().indexOf(name.toLowerCase()) > -1 ||
                !IsStringValueNullOrEmpty(participant.user_search_key) && participant.user_search_key.trim().toLowerCase().indexOf(partWoDia) > -1) {
                searchParticipants.push(participant);
            }

        });
        return searchParticipants;

    };

        
});