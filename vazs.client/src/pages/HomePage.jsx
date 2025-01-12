import {Link} from "react-router-dom";
import "./HomePage.scss";
const HomePage = () => {
  return (
    <div className="home">
      <nav>
        <Link className="item__link" to={HomePage}>
          <span className="logo">
            vazs <br />
            client
          </span>
        </Link>
        <ul className="nav">
          <li className="nav__item">
            <Link to="/home" className="item__link">Главная</Link>
          </li>
          <li className="nav__item">
            <Link to="/login" className="item__link">
              <div className="register-link">Войти</div>
            </Link>
          </li>
        </ul>
      </nav>
      <header>
        <h1 className="header__title">Кто мы?</h1>
        <div className="header__text">
          Здесь текст 
        </div>
      </header>
      <main>
        <h1 className="main__title">Чем мы<br/>занимаемся?</h1>
        <div className="main__text">
          Здесь текст 
        </div>
      </main>
      <section className="about">in progress</section>
    </div>
  );
}

export default HomePage