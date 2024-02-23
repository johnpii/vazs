import { useState } from "react";
import { Link } from "react-router-dom";
import { login } from "../services/auth.service";
import "./LoginPage.scss"

const LoginPage = () => {
  const [email, setEmail] = useState(null);
  const [password, setPassword] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    login(email, password);
  };
  return (
    <div className="login">
      <div className="container">
        <h5 className="login__title">Войти в аккаунт</h5>
        <form className="login__form">
          <div className="login__email">
            <input
              className="email__input"
              type="email"
              name="email"
              placeholder="Email"
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>
          <div className="login__password">
            <input
              className="password__input"
              type="password"
              name="password"
              placeholder="Пароль"
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
          <button className="form__submit" type="submit" onClick={handleSubmit}>
            Войти
          </button>
        </form>
        <div className="no-account">
          <p>Нет аккаунта?</p>
          <Link to="/register" className="register-link">
            Зарегистрироваться
          </Link>
        </div>
      </div>
    </div>
  );
}

export default LoginPage