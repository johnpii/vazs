import { useState } from "react"
import { register } from "../services/auth.service";
const RegisterPage = () => {
  const [email, setEmail] = useState(null);
  const [password, setPassword] = useState(null);
  const [nickname, setNickname] = useState(null);

  const handleSubmit = async(e) => {
    e.preventDefault();
    await register(email,password,nickname)
  }
  return (
    <div className="register">
      <h1 className="register__title">Регистрация</h1>
      <form className="register__form">
        <div className="register__email">
          <input
            className="email__input"
            type="email"
            name="email"
            placeholder="Email"
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>
        <div className="register__password">
          <input
            className="password__input"
            type="password"
            name="password"
            placeholder="Пароль"
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <div className="register__nickname">
          <input
            className="nickname__input"
            type="text"
            name="nickname"
            placeholder="Имя пользователя"
            onChange={(e) => setNickname(e.target.value)}
          />
        </div>
        <button
          className="form__submit"
          type="submit"
          onClick={handleSubmit}
        >
          Зарегистрироваться
        </button>
      </form>
    </div>
  );
}

export default RegisterPage