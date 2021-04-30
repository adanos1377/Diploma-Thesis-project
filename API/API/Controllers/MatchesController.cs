using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{   //hard coded root - api/Matching
    [Route("api/Matches")]
    [ApiController]
    public class matches : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILiteRepo _repository;
        private readonly IPoczekalnia _poczekalnia;
        private readonly IConfig _config;
        private readonly int configID = 1;

        public matches(ILiteRepo repo,IMapper mapper, IPoczekalnia poczekalnia, IConfig config)
        {
            _mapper = mapper;
            _repository = repo;
            _poczekalnia = poczekalnia;
            _config = config;
        }

        //GET api/Matching
        [HttpGet]
        public ActionResult test([FromBody] string nick) //zwraca dane serwera do gry
        {
            return NoContent();
        }

        //POST api/Matching
        [Route("solo")]
        [HttpPost]//tworzenie
        public async Task<int> CreateMatchSolo(MatchSoloCreate matchsolo) //zapisanie informacji o rozpoczętym meczu
        {
            int matchID = await _repository.Add_Game_Solo(matchsolo.players,matchsolo.ranked);
            _poczekalnia.remove_lobby_from_pool(matchsolo.lobby_id);
            return matchID;
            //return CreatedAtRoute(nameof(GetPlayerInfo), new { nick = playerReadModel.Nickname }, playerReadModel);
        }
        [Route("team")]
        [HttpPost]//tworzenie
        public async Task<int> CreateMatchTeam(MatchTeamCreate matchteam) //zapisanie informacji o rozpoczętym meczu
        {
            //async Task<ActionResult<PlayerReadDTO>>
            int matchID = await _repository.Add_Game_Team(matchteam.teams, matchteam.ranked);
            _poczekalnia.remove_lobby_from_pool(matchteam.lobby_id);
            return matchID;
            //zapisz mecz do bazy danych i uaktualnij statystyki graczy
            //return CreatedAtRoute(nameof(GetPlayerInfo), new { nick = playerReadModel.Nickname }, playerReadModel);
        }
        [HttpPost("{lobby_id}")]
        public ActionResult LobbyRefill(string lobby_id, [FromBody] Lobby NotFullLobby)
        {
            //async Task<ActionResult<PlayerReadDTO>>

            _poczekalnia.add_lobby_to_pool(NotFullLobby);
            return Ok();
            //return CreatedAtRoute(nameof(GetPlayerInfo), new { nick = playerReadModel.Nickname }, playerReadModel);
        }
        //PUT api/Matching/{id}
        [Route("solo")]
        [HttpPut]//tworzenie
        public async Task<ActionResult> FinishMatchSolo(SoloScoresDTO soloScores)
        {
            var x = await _repository.Add_Result_Solo(soloScores.match_id, soloScores.scores,configID);
            return Ok();
        }
        [Route("team")]
        [HttpPut]//tworzenie
        public async Task<ActionResult> FinishMatchTeam(TeamScoresDTO teamScores)
        {
            var x = await _repository.Add_Result_Team(teamScores.match_id, teamScores.scores, configID);
            return Ok();
        }
        //PATH api/Matching/{nick}
        [HttpPatch("{nick}")] //update części
        public ActionResult PatchCommand(string nick, JsonPatchDocument<PlayerUpdateDTO> patch)
        {
            return NoContent();
        }
        //DELETE api/Matching/{nick}
        [HttpDelete("{matchID}")] //usuwanie
        public ActionResult DeleteCommand(int matchID)
        {
            return NoContent();
        }


    }
}
