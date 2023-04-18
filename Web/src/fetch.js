
const url = "https://localhost:7198/Message/graph"


async function sequenceDiagram(searchHeader, url) {
    const params = new URLSearchParams({
        header: searchHeader
    });

    const request = `${url}?${params}`
    await fetch(request, {

        headers: {
            'Access-Control-Allow-Origin':'*'
        }}).then(response => {
            return json = response.json()
        })
        .then (obj => {
            const sss = JSON.parse(sample())
            drawDiagram(obj, document.getElementById('paper'))
        }
    )
        .catch(ERR => {
            console.log(ERR)
        })
}

document.getElementById('input-header').onclick =  async function () {
    let h = document.getElementById('search-header').value
    let start = performance.now()
    await sequenceDiagram(h, url)
    let finish = performance.now()
    document.getElementById("search-time").textContent = ((finish-start)/1000).toFixed(3).toString() + "s"
}