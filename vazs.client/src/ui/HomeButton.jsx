import { Link } from "react-router-dom";
import "./HomeButton.scss";

const HomeButton = () => {
  return (
    <div className="home-button">
      <Link to="/home" className="home-button__link">
        На главную
      </Link>
    </div>
  );
};

export default HomeButton;
