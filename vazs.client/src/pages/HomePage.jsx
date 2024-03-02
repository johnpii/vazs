import {Link} from "react-router-dom";
import "./HomePage.scss";
const HomePage = () => {
  return (
    <div className="home">
      <nav>
        <Link className="item__link" to={HomePage}>
          <span className="logo">
            Studio <br />
            logo
          </span>
        </Link>
        <ul className="nav">
          <li className="nav__item">
            <Link className="item__link">Главная</Link>
          </li>
          <li className="nav__item">
            <Link className="item__link">
              <div className="register-link">Войти</div>
            </Link>
          </li>
        </ul>
      </nav>
      <header>
        <h1 className="header__title">Кто мы?</h1>
        <div className="header__text">
          Здесь текст про то, что мы студия крутых разрабов
        </div>
      </header>
      <main>
        <h1 className="main__title">Чем мы<br/>занимаемся?</h1>
        <div className="main__text">
          Здесь текст про то, что мы делаем крутые сайты
        </div>
      </main>
      <section className="about">in progress</section>
    </div>
  );
}

export default HomePage