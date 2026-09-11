const groups = {};

data.forEach(function (row) {
  const key = row.name + '|' + row.floo_id;
  if (!groups[key]) {
    groups[key] = {
      name: row.name,
      floo_id: row.floo_id
    };
  }
  groups[key][row.tr] = {
    pro: row.pro,
    isconnected: row.isconnected,
    count: row.count
  };
});

return Object.values(groups);
