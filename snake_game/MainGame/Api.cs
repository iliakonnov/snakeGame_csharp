using snake_game.Bonuses;

namespace snake_game.MainGame
{
    
    public partial class MainGame
    {
        private class GameEvents : Accessable
        {
            public bool Invulnerable;
            public int Damage;

            public override TResult GetProperty<TResult>(string propertyName)
            {
                switch (propertyName)
                {
                    case nameof(Invulnerable):
                        return (TResult) (object) Invulnerable;
                    case nameof(Damage):
                        return (TResult) (object) Damage;
                    default:
                        return base.GetProperty<TResult>(propertyName);
                }
            }
        }
        
        private int _damage;
        
        /// <summary>
        /// Уменьшает кол-во жизней змеи
        /// </summary>
        /// <param name="damage">На сколько уменьшить. Может быть &lt; 0</param>
        public void Damage(int damage)
        {
            _lives -= damage;
            _damagedTime = _gameTime;
            _damage = damage;
        }

        public void Score(int score)
        {
            _score += score;
            if (_score % _config.GameConfig.ScoreToLive == 0)
            {
                _lives += 1;
            }
        }
    }
}