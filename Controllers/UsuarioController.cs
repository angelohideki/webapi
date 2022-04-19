using Microsoft.AspNetCore.Mvc;
using webapi.Model;
using webapi.Repository;
using System.Text.RegularExpressions;
using System;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;
        public UsuarioController(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _repository.BuscaUsuarios();
            return usuarios.Any() ? Ok(usuarios) : NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _repository.BuscaUsuario(id);
            return usuario != null ? Ok(usuario) : NotFound("Usuário não encontrado");
        }

        [HttpPost]
        public async Task<IActionResult> Post(Usuario usuario)
        {
            _repository.AdicionaUsuario(usuario);
            return await _repository.SaveChangesAsync()
                    ? Ok("Usuário Adicionado com sucesso")
                    : BadRequest("Erro ao salvar o usuário");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Usuario usuario)
        {
            var usuarioBanco = await _repository.BuscaUsuario(id);
            var senha = usuario.Senha;
            int tamanhoSenha = senha.Length;
            int qtdeNumeros = GetDigitos(senha);
            int qtdeMinusculas = GetMinusculas(senha);
            int qtdeMaiusculas = GetMaiusculas(senha);
            int qtdeSimbolos = GetSimbolos(senha);
            if (tamanhoSenha < 8 || tamanhoSenha > 12) return BadRequest("A senha deve ter no mínimo 8 e no máximo 12 caracteres");
            if (usuarioBanco == null) return NotFound("Usuário não encontrado");
            if (qtdeNumeros == 0) return BadRequest("A senha precisa ter pelo menos um número.");
            if (qtdeMinusculas == 0) return BadRequest("A senha precisa ter pelo menos uma letra minúscula.");
            if (qtdeMaiusculas == 0) return BadRequest("A senha precisa ter pelo menos uma letra maiúscula.");
            if (qtdeSimbolos == 0) return BadRequest("A senha precisa ter pelo menos um caracter especial.");
            if (ValidaFormatoEmail(usuario.Email) == false) return BadRequest("Digite um email válido.");
            usuarioBanco.Nome = usuario.Nome ?? usuarioBanco.Nome;
            usuarioBanco.Email = usuario.Email ?? usuarioBanco.Email;
            usuarioBanco.Telefone = usuario.Telefone ?? usuarioBanco.Telefone;
            usuarioBanco.Senha = usuario.Senha ?? usuarioBanco.Senha;
            usuarioBanco.Ativo = usuario.Ativo != usuarioBanco.Ativo ? usuario.Ativo : usuarioBanco.Ativo;

            _repository.AtualizaUsuario(usuarioBanco);
            return await _repository.SaveChangesAsync()
                    ? Ok("Usuário atualizado com sucesso")
                    : BadRequest("Erro ao atualizar o usuário");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioBanco = await _repository.BuscaUsuario(id);
            if (usuarioBanco == null) return NotFound("Usuário não encontrado");

            _repository.DeletaUsuario(usuarioBanco);
            return await _repository.SaveChangesAsync()
                    ? Ok("Usuário removido com sucesso")
                    : BadRequest("Erro ao remover o usuário");
        }

        private bool ValidaFormatoEmail(string email)
        {
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            return rg.IsMatch(email);
        }
        private int GetDigitos(string senha)
        {
            int qtde = senha.Length - Regex.Replace(senha, "[0-9]", "").Length;
            return qtde;
        }
        private int GetMaiusculas(string senha)
        {
            int qtde = senha.Length - Regex.Replace(senha, "[A-Z]", "").Length;
            return qtde;
        }
        private int GetMinusculas(string senha)
        {
            int qtde = senha.Length - Regex.Replace(senha, "[a-z]", "").Length;
            return qtde;
        }
        private int GetSimbolos(string senha)
        {
            int qtde = Regex.Replace(senha, "[a-zA-Z0-9]", "").Length;
            return qtde;
        }

    }
}