const recipes = document.querySelector('.recipes');

document.addEventListener('DOMContentLoaded', function() {
    //add right nav
    const menus = document.querySelectorAll('.side-menu');
    M.Sidenav.init(menus, { edge: 'right' });
    //add left nav
    const forms = document.querySelectorAll('.side-form');
    M.Sidenav.init(forms, { edge: 'left' });
});

// render recipe data
const renderRecipe = (data, id) => {

  const html = `
    <div class="card-panel recipe white row" data-id="${id}">
      <img src="/img/dish.png" alt="recipe thumb">
      <div class="recipe-details">
        <div class="recipe-title">${data.name}</div>
        <div class="recipe-ingredients">${data.ingredients}</div>
      </div>
      <div class="recipe-delete">
        <i class="material-icons" data-id="${id}">delete_outline</i>
      </div>
    </div>
  `;
  recipes.innerHTML += html;

};

// remove recipe
const removeRecipe = (id) => {
  const recipe = document.querySelector(`.recipe[data-id='${id}']`);
  recipe.remove();
};