const url = "https://localhost:7198/Message/graph"


async function sequenceDiagram(header, value, date, url) {
    const params = new URLSearchParams({
        header: header,
        value: value,
        date: date
    });

    const request = `${url}?${params}`
    await fetch(request, {

        headers: {
            'Access-Control-Allow-Origin': '*'
        }
    }).then(response => {
        return json = response.json()
    })
        .then(obj => {
                drawDiagram(obj, document.getElementById('paper'))
            }
        )
        .catch(ERR => {
            console.log(ERR)
        })
}

document.getElementById('search-button').onclick = async function () {
    let header = document.getElementById('search-header').value
    let value = document.getElementById('search-value').value
    // let from = document.getElementById('search-from').value
    // let to = document.getElementById('search-to').value

    let date = document.getElementById('search-date').value

    let start = performance.now()
    await sequenceDiagram(header, value, date, url)
    let finish = performance.now()
    document.getElementById("search-time").textContent = ((finish - start) / 1000).toFixed(3).toString() + "s"
}

let array = ["Unspecified", "Call-Id"];
let selectList = document.getElementById('search-header')
for (let i = 0; i < array.length; i++) {
    let option = document.createElement("option");
    option.value = array[i];
    option.text = array[i];
    selectList.appendChild(option);
}
