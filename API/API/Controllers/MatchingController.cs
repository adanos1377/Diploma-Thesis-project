using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Models;
using API.Atribute;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{   //hard coded root - api/Matching
    
    [Route("api/Matching")]
    [ApiController]
    //[ApiKeyAtribute]
    public class matching : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILiteRepo _repository;
        private readonly IPoczekalnia _poczekalnia;
        private readonly IConfig _config;
        private readonly int configID=1;
        public matching(ILiteRepo repo, IMapper mapper, IPoczekalnia poczekalnia, IConfig config)
        {
            _mapper = mapper;
            _repository = repo;
            _poczekalnia = poczekalnia;
            _config = config;

        }

        //GET api/Matching
        [Route("Match/{NickName}")]
        [HttpGet]
        public async Task<ActionResult<Server>> GetMatch(string NickName) //PlayerMatch p   //zwraca dane serwera do gry  [FromBody] string nick
        {
            //var players = _repository.MatchPlayers();//To Be Done
            //return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commanditems));
            //return Ok(_mapper.Map<IEnumerable<PlayerReadDTO>>(players));

            GameConfigs gameConfig = await _config.GetConfig(configID);
            var player = await _repository.GetPlayerInfo(NickName); // p.NickName
            if (player == null)
            {
                return NotFound();
            }
            else
            {
                //'find him a place in queue'
                //'add him to queue'
                Lobby lobby = _poczekalnia.find_lobby(player, gameConfig);
                //'send him server adress and lobby id'

                return (new Server(lobby.lobby_ID, gameConfig));
            }

        }
        //GET api/Matching/{nick}
        [HttpGet("{nick}", Name = "GetPlayerInfo")]
        public async Task<ActionResult<PlayerReadDTO>> GetPlayerInfo(string nick) //funkcja do testów
        {
            var playerinfo = await _repository.GetPlayerInfo(nick);
            //var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            GameConfigs gameConfig = await _config.GetConfig(configID);


            if (playerinfo != null)
            {
                //bez if zwraca 204 no content w przypadku braku wyniku
                //return Ok(_mapper.Map<CommandReadDTO>(commanditem));
                var player=_mapper.Map<PlayerReadDTO>(playerinfo);
                player.ApiName = gameConfig.Name;
                return player;
            }
            //zwraca 404 not found
            return NotFound();


            //return Ok(nick);
        }
        //POST api/Matching
        [HttpPost]//tworzenie
        public async Task<ActionResult<PlayerReadDTO>> CreatePlayer(PlayerCreateDTO playerCreate) //dodawanie gracza to bazy danych
        {
            Player response;
            GameConfigs gameConfig = await _config.GetConfig(configID);
            var playerModel = _mapper.Map<Player>(playerCreate);
            try
            {
                response = await _repository.AddPlayer(playerModel, configID);
            }
            catch (Exception ex)
            {
                return BadRequest("Nick jest już zajęty");
            }
            var playerReadModel = _mapper.Map<PlayerReadDTO>(response);
            playerReadModel.ApiName = gameConfig.Name;
            //return Ok(_mapper.Map<PlayerReadDTO>(playerModel)); 
            return CreatedAtRoute(nameof(GetPlayerInfo), new { nick = playerReadModel.Nickname }, playerReadModel);
            //return NoContent();
        }
        //PUT api/Matching/{id}
        [HttpPut("{id}")] //update całości
        public ActionResult UpdateCommand(int id, PlayerUpdateDTO update)
        {
            //var commandModel = _repository.GetcommandbyID(id);
            //if (commandModel == null)
            //{
            //    return NotFound();
            //}
            //_mapper.Map(update, commandModel);
            //_repository.UpdateCommand(commandModel);
            //_repository.SaveChanges();
            return NoContent();//standard zwracany przez put
        }
        //PATH api/Matching/{nick}
        [HttpPatch("{nick}")] //update nicku
        public ActionResult PatchCommand(string nick, JsonPatchDocument<PlayerUpdateDTO> patch)
        {
            //var commandModel = _repository.GetcommandbyID(id);
            //if (commandModel == null)
            //{
            //    return NotFound();
            //}
            //var commandPatch = _mapper.Map<CommandUpdateDTO>(commandModel);
            //patch.ApplyTo(commandPatch, ModelState);
            //if (!TryValidateModel(commandPatch))
            //{
            //    return ValidationProblem(ModelState);
            //}
            ////spowrotem na model
            //_mapper.Map(commandPatch, commandModel);
            //_repository.UpdateCommand(commandModel);
            //_repository.SaveChanges();
            return NoContent();
        }
        //DELETE api/Matching/{nick}
        [HttpDelete("{nick}")] //usuwanie
        public ActionResult CancelMatchmaking(string nick,[FromBody]string lobby_id)
        {
            bool result=_poczekalnia.remove_player_from_lobby(nick, lobby_id);
            if (result==true)
            {
                return Ok();
            }
            else
            {
                return NoContent();
            }
            
        }


    }
}
